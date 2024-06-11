using System;
using Mirror;
using UnityEngine;

[System.Serializable]
public class ClientPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNicknameChangedEvent))]
    public string nickname;

    public LobbySystem lobbySystem;

    public ServerData serverPlayer;

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Find the lobby system
        lobbySystem = FindObjectOfType<LobbySystem>();

        if (lobbySystem != null) { lobbySystem.AddClientPlayer(this, addNicknameListener: ClientCanEdit(), addStartGameListener: ClientIsServerOwner()); }
    }

    [TargetRpc]
    public void SetServerPlayer(NetworkConnectionToClient connection, string playerData)
    {
        serverPlayer = ServerData.Deserialize(playerData);

    }



    [Server]
    public NetworkConnectionToClient GetConnectionToClient()
    {
        return connectionToClient;
    }
    [Client]
    public NetworkConnection GetConnectionToServer()
    {
        return connectionToServer;
    }

    [ClientRpc]
    public void RPC_RemoveClientLobbyUI()
    {

        if (lobbySystem != null)
        {
            lobbySystem.CheckForLostPlayers();
        }
        else
        {
            Debug.LogError("Could not find lobby system to remove client player from.");
        }
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        nickname = $"Player {netId}";

        DontDestroyOnLoad(this);
    }

    [Command]
    public void CmdSetNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
        {
            return;
        }

        this.nickname = nickname;

    }

    [Server]
    public bool CanEdit()
    {
        return connectionToClient.identity == NetworkClient.connection.identity;
    }

    [Server]
    public bool IsServerOwner()
    {
        return GameCore.Instance.IsServerOwner(connectionToClient);
    }

    [Client]
    public bool ClientIsServerOwner()
    {
        return isServer && isLocalPlayer;
    }

    [Client]
    public bool ClientCanEdit()
    {
        return isLocalPlayer;
    }

    [Client]
    private void OnNicknameChangedEvent(string old, string newNickname)
    {

        if (newNickname == null || newNickname == string.Empty)
        {
            ////Debug.LogWarning("Nickname is null or empty");
            return;
        }
        if (lobbySystem == null)
        {
            ////Debug.LogWarning("LobbySystem not found when trying to update nickname.");
            return;
        }
        lobbySystem.UpdateClientPlayerNickname(this, newNickname);

    }

    [Command]
    public void CmdMoveUnits(UnitGroup selectedUnits, Vector2 realPoint)
    {
        // Check if the player has authority over the selected units
        Debug.Log("Moving units on server");

        selectedUnits.SetGoalPosition(realPoint, 2.0f);
    }

    [Command]
    public void CmdSpawnBuilding(string buildingSettings, Vector2 vector2, NetworkConnectionToClient connection = null)
    {

        // Check if the player is talking with the right clientPlayer
        if (connection != null && connection.identity != connectionToClient.identity)
        {
            Debug.LogWarning("Player is trying to spawn a building for another player.");
            return;
        }

        if (buildingSettings == null)
        {
            return;
        }

        GameCore.Instance.SpawnBuilding(connectionToClient, buildingSettings, vector2);
    }

}