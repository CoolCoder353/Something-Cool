using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum HeuristicType
{
    Manhattan,
    Euclidean,
    Chebyshev,
    Octile,
    SquaredEuclidean,
    BaseOne
}
public class PathObject
{
    public Vector2Int start;
    public Vector2Int end;
    public List<Vector2Int> path;

    public PriorityQueue<TileData> openSet;
    public PriorityQueue<TileData> closedSet;

    public PathObject()
    {
        path = new List<Vector2Int>();
    }

    public PathObject(PathObject other)
    {
        start = other.start;
        end = other.end;
        path = new List<Vector2Int>(other.path);
        openSet = new PriorityQueue<TileData>(other.openSet);
        closedSet = new PriorityQueue<TileData>(other.closedSet);
    }
}

public static class AStar
{
    public static PathObject FindPath(Dictionary<Vector2Int, TileData> grid, Vector2Int start, Vector2Int end, HeuristicType heuristicType = HeuristicType.Octile, bool debug = false)
    {
        TileData startTile = grid[start];
        TileData endTile = grid[end];

        startTile.costScore = 0;

        if (!startTile.IsWalkable || !endTile.IsWalkable)
        {
            Debug.LogError("Start or end tile is unwalkable");
            return null;
        }

        PathObject path = new PathObject();
        path.start = start;
        path.end = end;

        PriorityQueue<TileData> openSet = new PriorityQueue<TileData>(grid.Count * 3, (x, y) => x.costScore.CompareTo(y.costScore));
        PriorityQueue<TileData> closedSet = new PriorityQueue<TileData>(grid.Count * 3, (x, y) => x.costScore.CompareTo(y.costScore));

        openSet.Enqueue(startTile);

        while (openSet.Count > 0)
        {
            TileData currentTile = openSet.Dequeue();

            if (debug) Debug.Log("Current Tile: " + currentTile.position + " End Tile: " + endTile.position);


            if (currentTile.position == end)
            {
                if (debug) Debug.Log("Found exit, retracing path");
                path.path = RetracePath(startTile, endTile);
                path.openSet = openSet;
                path.closedSet = closedSet;
                return path;
            }

            if (debug) Debug.Log($"Adding current tile to closed set: {currentTile.position}");

            closedSet.Enqueue(currentTile);

            foreach (Vector2Int neighbourPosition in GetNeighbours(currentTile.position))
            {
                if (!grid.ContainsKey(neighbourPosition))
                {
                    if (debug) Debug.Log($"Neighbour position {neighbourPosition} is not in the grid");
                    continue;
                }

                TileData neighbourTile = grid[neighbourPosition];

                if (debug) Debug.Log($"Neighbour Tile: {neighbourTile.position}");

                if (!neighbourTile.IsWalkable || closedSet.Contains(neighbourTile))
                {
                    if (debug) Debug.Log($"Neighbour tile is unwalkable ({!neighbourTile.IsWalkable}) or already in closed set ({closedSet.Contains(neighbourTile)})");
                    continue;
                }
                float HCost = 0;
                switch (heuristicType)
                {
                    case HeuristicType.Manhattan:
                        HCost = CalculateHCost_Manhattan(neighbourTile.position, endTile.position);
                        break;
                    case HeuristicType.Euclidean:
                        HCost = CalculateHCost_Euclidean(neighbourTile.position, endTile.position);
                        break;
                    case HeuristicType.Chebyshev:
                        HCost = CalculateHCost_Chebyshev(neighbourTile.position, endTile.position);
                        break;
                    case HeuristicType.Octile:
                        HCost = CalculateHCost_Octile(neighbourTile.position, endTile.position);
                        break;
                    case HeuristicType.SquaredEuclidean:
                        HCost = CalculateHCost_SquaredEuclidean(neighbourTile.position, endTile.position);
                        break;
                    case HeuristicType.BaseOne:
                        HCost = 1;
                        break;
                }

                float newMovementCostToNeighbour = CalculateGCost(neighbourTile.position, startTile.position) + HCost + neighbourTile.costScore;

                newMovementCostToNeighbour *= -1;

                if (!openSet.Contains(neighbourTile))
                {
                    neighbourTile.costScore = newMovementCostToNeighbour;
                    neighbourTile.parent = currentTile;

                    if (!openSet.Contains(neighbourTile))
                    {
                        openSet.Enqueue(neighbourTile);
                    }
                }
            }
        }




        return null;
    }


    public static List<Vector2Int> RetracePath(TileData startTile, TileData endTile)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        TileData currentTile = endTile;

        while (currentTile != startTile)
        {
            path.Add(currentTile.position);
            currentTile = currentTile.parent;
        }

        path.Reverse();
        return path;
    }

    public static List<Vector2Int> GetNeighbours(Vector2Int position)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>
        {
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y),
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1),
            new Vector2Int(position.x + 1, position.y + 1),
            new Vector2Int(position.x - 1, position.y - 1),
            new Vector2Int(position.x + 1, position.y - 1),
            new Vector2Int(position.x - 1, position.y + 1)
        };

        return neighbours;
    }

    /// <summary>
    /// Calculate the heuristic cost (HCost) using Manhattan distance.
    /// </summary>
    public static float CalculateHCost_Manhattan(Vector2Int currentPosition, Vector2Int endPosition)
    {
        return Mathf.Abs(currentPosition.x - endPosition.x) + Mathf.Abs(currentPosition.y - endPosition.y);
    }

    /// <summary>
    /// Calculate the heuristic cost (HCost) using Euclidean distance.
    /// </summary>
    public static float CalculateHCost_Euclidean(Vector2Int currentPosition, Vector2Int endPosition)
    {
        return Mathf.Sqrt(Mathf.Pow(currentPosition.x - endPosition.x, 2) + Mathf.Pow(currentPosition.y - endPosition.y, 2));
    }

    /// <summary>
    /// Calculate the heuristic cost (HCost) using Chebyshev distance.
    /// </summary>
    public static float CalculateHCost_Chebyshev(Vector2Int currentPosition, Vector2Int endPosition)
    {
        return Mathf.Max(Mathf.Abs(currentPosition.x - endPosition.x), Mathf.Abs(currentPosition.y - endPosition.y));
    }

    /// <summary>
    /// Calculate the heuristic cost (HCost) using Octile distance.
    /// Octile distance is a more accurate heuristic for grid-based pathfinding where diagonal movement is allowed.
    /// It's a combination of Manhattan and Chebyshev distances.
    /// </summary>
    public static float CalculateHCost_Octile(Vector2Int currentPosition, Vector2Int endPosition, float CostForMoving = 1, float CostForMovingDiagonaly = 1.41421356237f)
    {
        float dx = Mathf.Abs(currentPosition.x - endPosition.x);
        float dy = Mathf.Abs(currentPosition.y - endPosition.y);
        return CostForMoving * (dx + dy) + (CostForMovingDiagonaly - 2 * CostForMoving) * Mathf.Min(dx, dy);
    }


    /// <summary>
    /// Calculate the heuristic cost (HCost) using Squared Euclidean distance.
    /// This is a variant of Euclidean distance that avoids the expensive square root operation.
    /// It's not an admissible heuristic for A* (it can overestimate the cost), but it can be faster and still gives reasonable results in many cases.
    /// </summary>
    public static float CalculateHCost_SquaredEuclidean(Vector2Int currentPosition, Vector2Int endPosition)
    {
        return Mathf.Pow(currentPosition.x - endPosition.x, 2) + Mathf.Pow(currentPosition.y - endPosition.y, 2);
    }

    /// <summary>
    /// Calculate the movement cost (GCost) from the start node to the current node.
    /// </summary>
    public static float CalculateGCost(Vector2Int currentPosition, Vector2Int startNode, int weight = 1)
    {
        return Mathf.Abs(currentPosition.x - startNode.x) + Mathf.Abs(currentPosition.y - startNode.y) * weight;
    }

}