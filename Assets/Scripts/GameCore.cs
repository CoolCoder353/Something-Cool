using Mirror;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// The GameCore class is responsible for managing the server's game state.
/// It inherits from Mirror's NetworkBehaviour class.
/// </summary>
public class GameCore : NetworkBehaviour
{
    /// <summary>
    /// Singleton instance of the GameCore. 
    /// Ensures that only one GameCore exists in the scene at any time.
    /// </summary>
    public static GameCore Instance { get; private set; }

    /// <summary>
    /// List of ServerPlayer objects representing the players on the server.
    /// </summary>
    public Dictionary<NetworkIdentity, ServerPlayer> ServerPlayers = new Dictionary<NetworkIdentity, ServerPlayer>();

    /// <summary>
    /// NetworkConnection object representing the owner of the server.
    /// </summary>
    private NetworkConnection serverOwner;

    private Building_Settings[] buildingsData;


    /// <summary>
    /// A preloaded list of all building settings in the game.
    /// </summary>
    public List<BuildingSettings> serverBuildingSettings;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// It initializes the singleton instance.
    /// </summary>
    public void Awake()
    {
        TIM.Console.Log($"GameCore Awake", TIM.MessageType.Network);

        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);

    }




    //<summary>
    //Sends the individual player's server player object to each client
    [Server]
    private void UpdateClientsPrivateData()
    {
        foreach (var player in ServerPlayers)
        {
            ClientPlayer clientPlayer = player.Key.GetComponent<ClientPlayer>();
            ////Debug.Log($"Server sending private data to {clientPlayer.GetConnectionToClient().connectionId} ({player.Value.data.Serialize()}, R:{player.Value.data.resources})");

            clientPlayer.SetServerPlayer(clientPlayer.GetConnectionToClient(), player.Value.data.Serialize());
        }
    }



    [ServerCallback]
    public void LateUpdate()
    {
        //TODO: Change this to 1 as a safe guard. For testing purposes it is set to 0
        if (ServerPlayers.Count > 0)
        {
            UpdateClientsPrivateData();
        }

    }

    [Server]
    public void SpawnBuilding(NetworkConnectionToClient conn, string buildingName, Vector2 position)
    {
        //Lookup in the BuildingSettings folder in Resources to see if the building can be built
        BuildingSettings buildingSettings = serverBuildingSettings.Find(x => x.buildingName == buildingName);
        if (buildingSettings == null)
        {
            Debug.Log("Building not found in BuildingSettings folder.");
            return;
        }
        //Try get the buildings TileMap
        if (ServerPlayers[conn.identity].data.resources < buildingSettings.cost)
        {
            Debug.Log("Player does not have enough resources to build this building.");
            return;
        }

        Tilemap tilemap = GameObject.FindGameObjectWithTag("BuildingMap").GetComponent<Tilemap>();
        //Check if the tilemap is null
        if (tilemap == null)
        {
            Debug.Log("Tilemap not found.");
            return;
        }
        //Get the tilemap's bounds
        BoundsInt bounds = tilemap.cellBounds;
        //Check if the position is within the bounds
        if (bounds.Contains(new Vector3Int((int)position.x, (int)position.y, 0)) == false)
        {
            Debug.Log("Position is not within the bounds of the tilemap.");
            return;
        }
        //Check if the tilemap has a tile at the position
        if (tilemap.HasTile(new Vector3Int((int)position.x, (int)position.y, 0)) == true)
        {
            Debug.Log("Tilemap already has a tile at the position. Must be empty to build a building.");
            return;
        }
        //Add the tile to the tilemap
        ServerPlayers[conn.identity].RemoveResources(buildingSettings.cost);

        TileMapSync.Instance.UpdateTile(new Vector3Int((int)position.x, (int)position.y, 0), buildingSettings.buildingName);
    }






    [Server]
    public void OnPlayerLeave(NetworkConnectionToClient conn)
    {
        //        Debug.Log($"Player {conn.connectionId} has disconnected");


        ServerPlayers.Remove(conn.identity);
        ServerPlayers.TrimExcess();

        //For each player still in the game, call the RPC_RemoveClientLobbyUI method
        foreach (var player in ServerPlayers)
        {
            ClientPlayer client = player.Key.GetComponent<ClientPlayer>();
            if (client.lobbySystem != null)
            {
                client.RPC_RemoveClientLobbyUI();
            }
        }



        // If the disconnected player was the server owner, set a new server owner
        if (IsServerOwner(conn))
        {

            //If the server owner was also the server -> kick all clients

            if (serverOwner.identity.isServer)
            {
                NetworkServer.Shutdown();
            }
            else
            {
                SetServerOwner(ServerPlayers.First().Key.connectionToClient);
            }


        }
    }



    /// <summary>
    /// Sets the owner of the server.
    /// </summary>
    [Server]
    public void SetServerOwner(NetworkConnectionToClient conn)
    {
        if (serverOwner != null)
        {
            serverOwner.identity.RemoveClientAuthority();
        }

        Debug.Log($"Player {conn.identity.netId} is now the server owner");
        serverOwner = conn;

        // Assign client authority to the new server owner
        conn.identity.AssignClientAuthority(conn);

        Debug.Log($"Server owner is now {conn.identity.netId}");
    }

    /// <summary>
    /// Checks if a NetworkConnection object is the owner of the server.
    /// </summary>
    [Server]
    public bool IsServerOwner(NetworkConnectionToClient conn)
    {
        return conn.identity == serverOwner.identity;
    }

    [Server]
    public void AddResourcesToPlayer(NetworkConnectionToClient conn, float amount)
    {
        ServerPlayers[conn.identity].AddResources(amount);
    }

    /// <summary>
    /// Command sent to the server to start the game.
    /// If the connection is the server owner, it changes the scene to the game scene.
    /// </summary>
    [Command(requiresAuthority = false)]
    public void Cmd_StartGame(NetworkConnectionToClient connection = null)
    {
        if (IsServerOwner(connection))
        {
            GameManager.Instance.ServerChangeScene("Map_1");
        }
    }
}