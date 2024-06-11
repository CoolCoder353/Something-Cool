using UnityEngine;


//A data object that represents a building

[CreateAssetMenu(fileName = "New Building", menuName = "Building")]
public class Building_Settings : ScriptableObject
{
    public string buildingName;

    public GameObject prefab;

    public int cost;

    public Sprite icon;


}