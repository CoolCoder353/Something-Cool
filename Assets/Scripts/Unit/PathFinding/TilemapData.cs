using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TilemapData", order = 1), Serializable]
public class TilemapData : ScriptableObject
{


    [ShowInInspector]
    public Dictionary<Vector2Int, TileData> tiles;
}

[Serializable]
public class TileData
{
    public Vector2Int position;
    public string tilemapName;
    public int sortingOrder;

    public bool IsWalkable;

    public float costScore = 1;

    public TileData parent;

    public TileData()
    {

    }

    public TileData(TileData other)
    {
        position = other.position;
        tilemapName = other.tilemapName;
        sortingOrder = other.sortingOrder;
        IsWalkable = other.IsWalkable;
        costScore = other.costScore;
        parent = other.parent;
    }
}