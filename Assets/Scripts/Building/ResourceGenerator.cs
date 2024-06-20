using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;

public class ResourceGenerator : Building
{

    public Vector2 resourceMinDirection;
    public int resourceMaxDistanceAway;

    public float resourceGenerationTime;
    public float resourceGenerationAmount;

    private int resourceDistance = 0;

    private bool resourceAvailable = false;

    private float lastResourceGenerationTime = 0;

    // Start is called before the first frame update
    [ServerCallback]
    void Start()
    {
        resourceMinDirection = resourceMinDirection.normalized;

        resourceAvailable = CheckIfResourceIsAvailable();
    }


    // Update is called once per frame
    [Server]
    public override void Tick(float deltaTime)
    {
        if (resourceAvailable)
        {
            if (Time.time - lastResourceGenerationTime > resourceGenerationTime)
            {
                lastResourceGenerationTime = Time.time;
                GameCore.Instance.AddResourcesToPlayer(netIdentity.connectionToClient, resourceGenerationAmount);
            }
        }

    }

    [Server]
    private bool CheckIfResourceIsAvailable()
    {
        Tilemap tilemap = TileMapSync.Instance.resourceTilemap;
        while (resourceDistance < resourceMaxDistanceAway)
        {
            Vector3Int resourcePosition = new Vector3Int((int)transform.position.x + (int)resourceMinDirection.x * resourceDistance, (int)transform.position.y + (int)resourceMinDirection.y * resourceDistance, 0);
            TileBase tile = tilemap.GetTile(resourcePosition);
            if (tile != null && !tile.name.ToLower().Contains("wall"))
            {
                return true;
            }

            resourceDistance++;
        }
        return false;

    }
}
