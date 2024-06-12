using UnityEngine;
using Mirror;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TileMapSync : NetworkBehaviour
{
    public static TileMapSync Instance { get; private set; }
    public Tilemap buildingTilemap;

    private List<BuildingSettings> buildingSettings;

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        buildingTilemap = GetComponent<Tilemap>();

        buildingSettings = new List<BuildingSettings>(Resources.LoadAll<BuildingSettings>("BuildingSettings"));


        Instance = this;
    }

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        buildingTilemap = GetComponent<Tilemap>();

        buildingSettings = new List<BuildingSettings>(Resources.LoadAll<BuildingSettings>("BuildingSettings"));


        Instance = this;
    }

    [Server]
    public void UpdateTile(Vector3Int position, string tilename)
    {
        if (!buildingSettings.Exists(x => x.buildingName == tilename))
        {
            Debug.LogError("Building not found.");
            return;
        }
        TileBase tile = buildingSettings.Find(x => x.buildingName == tilename).tile;



        buildingTilemap.SetTile(position, tile);
        int count = GetTileCount();

        //TODO: Make this different for each player based on what they can see. (Fog of War)
        foreach (NetworkConnection conn in NetworkServer.connections.Values)
        {
            UpdateTileMap(conn, position, tilename, count);
        }
    }

    [Server]
    public void UpdateTile(List<(Vector3Int, string)> tiles)
    {
        foreach ((Vector3Int position, string tilename) in tiles)
        {
            UpdateTile(position, tilename);
        }
    }


    [TargetRpc]
    public void UpdateTileMap(NetworkConnection connection, Vector3Int position, string tilename, int TileCount, bool checkCount = true)
    {
        if (!buildingSettings.Exists(x => x.buildingName == tilename))
        {
            Debug.LogError("Building not found.");
            return;
        }
        TileBase tile = buildingSettings.Find(x => x.buildingName == tilename).tile;

        buildingTilemap.SetTile(position, tile);

        Debug.Log("Updated tilemap");

        if (checkCount) CheckCount(TileCount);
    }

    [TargetRpc]
    public void UpdateTileMap(NetworkConnection connection, List<(Vector3Int, string)> tiles, int TileCount, bool checkCount = true)
    {
        foreach ((Vector3Int position, string tilename) in tiles)
        {
            UpdateTileMap(connection, position, tilename, TileCount, false);
        }

        if (checkCount) CheckCount(TileCount);
    }


    private bool CheckCount(int TileCount)
    {
        int count = 0;
        foreach (Vector3Int pos in buildingTilemap.cellBounds.allPositionsWithin)
        {
            if (buildingTilemap.GetTile(pos) != null)
            {
                count++;
            }
        }

        Debug.Log("Tile count: " + count);

        if (count == TileCount)
        {
            Debug.Log("All tiles are synced.");
            return true;
        }

        return false;
    }

    private int GetTileCount()
    {
        int count = 0;
        foreach (Vector3Int pos in buildingTilemap.cellBounds.allPositionsWithin)
        {
            if (buildingTilemap.GetTile(pos) != null)
            {
                count++;
            }
        }

        return count;
    }
}