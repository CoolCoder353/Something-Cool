using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "Building_Settings", menuName = "WAR/Building Settings", order = 1)]
public class BuildingSettings : ScriptableObject
{
    public string buildingName;
    public Sprite sprite;

    public TileBase tile;

    public int cost;


}