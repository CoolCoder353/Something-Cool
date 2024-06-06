using UnityEngine;


[CreateAssetMenu(fileName = "Game_Manager_Settings", menuName = "WAR/Settings", order = 1)]
public class GameManagerSettings : ScriptableObject
{
    public int initalResources = 100;
    public string coreBaseName = "Base";
}