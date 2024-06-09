using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using Mirror;



public class AStarManager : MonoBehaviour
{

    public static AStarManager Instance;

    public List<Tilemap> WalkableTilemaps;

    public List<Tilemap> UnwalkableTilemaps;

    public TilemapData Tiles;

    public HeuristicType heuristicType = HeuristicType.Octile;

    public bool showGizmos = true;

    [ShowIf("showGizmos")]
    public bool showPath = true;

    [ShowIf("showGizmos")]
    public bool showGrid = true;

    [ShowIf("showGizmos")]
    public bool showCostOfEachTile = true;


    [ShowIf("showPath")]
    public Vector2Int start;
    [ShowIf("showPath")]
    public Vector2Int end;

    [ShowIf("showPath")]
    public bool showPathData = true;


    private PathObject lastPathObject;
    private List<Vector2Int> path;


    [ServerCallback]
    private void Awake()
    {
        Instance = this;

        Bake();
    }

    [Button("ReCalculatePath"), ShowIf("showPath")]
    public void ReCalculatePath()
    {
        path = GetPath(start, end);
        Debug.Log("Updated path. Path length: " + path.Count);
    }


    [Button("Bake"), Server]
    public void Bake()
    {
        Instance = this;
        Dictionary<Vector2Int, TileData> visibleTiles = new Dictionary<Vector2Int, TileData>();
        // Iterate over walkable tilemaps
        foreach (Tilemap tilemap in WalkableTilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        Vector2Int localPlace = new Vector2Int(bounds.x + x, bounds.y + y);

                        if (!visibleTiles.ContainsKey(localPlace) || visibleTiles[localPlace].sortingOrder < renderer.sortingOrder)
                        {
                            TileData tileData = new TileData();
                            tileData.position = localPlace;
                            tileData.tilemapName = tilemap.name;
                            tileData.IsWalkable = true;
                            tileData.sortingOrder = renderer.sortingOrder;

                            visibleTiles[localPlace] = tileData;
                        }

                    }
                }
            }
        }

        // Iterate over unwalkable tilemaps
        foreach (Tilemap tilemap in UnwalkableTilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            TilemapRenderer renderer = tilemap.GetComponent<TilemapRenderer>();

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    TileBase tile = allTiles[x + y * bounds.size.x];
                    if (tile != null)
                    {
                        Vector2Int localPlace = new Vector2Int(bounds.x + x, bounds.y + y);

                        if (!visibleTiles.ContainsKey(localPlace) || visibleTiles[localPlace].sortingOrder < renderer.sortingOrder)
                        {
                            TileData tileData = new TileData();
                            tileData.position = localPlace;
                            tileData.tilemapName = tilemap.name;
                            tileData.IsWalkable = false;
                            tileData.sortingOrder = renderer.sortingOrder;

                            visibleTiles[localPlace] = tileData;
                        }

                        // After setting a tile as unwalkable, iterate over its neighbors
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                // Skip the tile itself
                                if (dx == 0 && dy == 0) continue;

                                Vector2Int neighborPlace = new Vector2Int(localPlace.x + dx, localPlace.y + dy);

                                // If the neighbor tile is walkable and exists in visibleTiles, set its cost to 12
                                if (visibleTiles.ContainsKey(neighborPlace) && visibleTiles[neighborPlace].IsWalkable)
                                {
                                    visibleTiles[neighborPlace].costScore = 12;
                                }
                            }
                        }
                    }
                }
            }
        }
        Tiles.tiles = visibleTiles;
    }


    public void OnDrawGizmosSelected()
    {
        if (!showGizmos)
        {
            return;
        }
        if (showGrid)
        {
            if (Tiles == null || Tiles.tiles == null)
            {
                return;
            }

            foreach (KeyValuePair<Vector2Int, TileData> tile in Tiles.tiles)
            {
                if (tile.Value.IsWalkable)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawWireCube(new Vector3(tile.Key.x, tile.Key.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), Vector3.one);
            }
        }
        if (showPath)
        {


            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(start.x, start.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), 0.3f);
            Gizmos.DrawSphere(new Vector3(end.x, end.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), 0.3f);

            if (path != null)
            {
                Gizmos.color = Color.blue;
                foreach (Vector2Int tile in path)
                {
                    if (tile != start && tile != end)
                    {
                        Gizmos.DrawSphere(new Vector3(tile.x, tile.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), 0.18f);
                    }

                }
            }
            if (lastPathObject != null && showPathData)
            {
                PathObject iteralble = new PathObject(lastPathObject);
                for (int i = 0; i < iteralble.openSet.Count; i++)
                {
                    TileData tile = iteralble.openSet.Dequeue();
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(new Vector3(tile.position.x, tile.position.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), 0.09f);
                    //Draw a number on the tile for its cost score
#if UNITY_EDITOR
                    UnityEditor.Handles.Label(new Vector3(tile.position.x, tile.position.y, gameObject.transform.position.z), tile.costScore.ToString());
#endif
                }

                for (int i = 0; i < iteralble.closedSet.Count; i++)
                {
                    TileData tile = iteralble.closedSet.Dequeue();
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(new Vector3(tile.position.x, tile.position.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), 0.09f);

#if UNITY_EDITOR    //Draw a number on the tile for its cost score
                    UnityEditor.Handles.Label(new Vector3(tile.position.x, tile.position.y, gameObject.transform.position.z), tile.costScore.ToString());
#endif
                }
            }
        }

        if (showCostOfEachTile)
        {
            if (Tiles == null || Tiles.tiles == null)
            {
                return;
            }

            foreach (KeyValuePair<Vector2Int, TileData> tile in Tiles.tiles)
            {
                if (tile.Value.IsWalkable)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
#if UNITY_EDITOR
                //Draw a number on the tile for its cost score
                UnityEditor.Handles.Label(new Vector3(tile.Key.x, tile.Key.y, gameObject.transform.position.z) + new Vector3(0.5f, 0.5f, 0), tile.Value.costScore.ToString());
#endif
            }
        }


    }

    [Server]
    public List<Vector2> GetPath(Vector2 start, Vector2 end, Vector2 offset = default)
    {
        Vector2Int roundedStart = new Vector2Int(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));
        Vector2Int roundedEnd = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));

        List<Vector2Int> roundedPath = GetPath(roundedStart, roundedEnd);

        if (offset == default)
        {
            offset = new Vector2(0.5f, 0.5f);
        }

        List<Vector2> path = new List<Vector2>(roundedPath.Count);
        for (int i = 0; i < roundedPath.Count; i++)
        {
            // If this tile is the start or end, use the exact position
            if (i == 0)
            {
                path.Add(start);
            }
            else if (i == roundedPath.Count - 1)
            {
                path.Add(end);
            }
            else
            {
                path.Add(new Vector2(roundedPath[i].x, roundedPath[i].y) + offset);
            }
        }
        return path;
    }

    [Server]
    public List<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        if (Tiles == null || Tiles.tiles == null)
        {
            Debug.LogWarning($"Tiles are null. Did you forget to bake the tiles?");
            return path;
        }

        if (!Tiles.tiles.ContainsKey(start) || !Tiles.tiles.ContainsKey(end))
        {
            Debug.LogWarning($"Start ({!Tiles.tiles.ContainsKey(start)}) or end ({!Tiles.tiles.ContainsKey(end)}) tile is not in the tilemap. Start:{start}, End: {end}");
            return path;
        }

        if (!Tiles.tiles[start].IsWalkable || !Tiles.tiles[end].IsWalkable)
        {
            Debug.LogWarning($"Start ({!Tiles.tiles[start].IsWalkable}) or end ({!Tiles.tiles[end].IsWalkable}) tile is not walkable. Start:{start}, End: {end}");
            return path;
        }

        Dictionary<Vector2Int, TileData> copy = new Dictionary<Vector2Int, TileData>();
        foreach (KeyValuePair<Vector2Int, TileData> tile in Tiles.tiles)
        {
            copy[tile.Key] = new TileData(tile.Value);
        }

        PathObject pathObject = AStar.FindPath(copy, start, end);
        path = pathObject.path;

        if (path.Count == 0)
        {
            Debug.LogWarning($"Could not find path from {start} to {end}");
            return null;
        }

        lastPathObject = pathObject;
        return path;
    }

}








