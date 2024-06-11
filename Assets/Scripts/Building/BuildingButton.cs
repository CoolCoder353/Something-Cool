using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class BuildingButton : MonoBehaviour
{
    public BuildingSettings buildingSettings;

    public Button button;
    public TMP_Text costText;
    public Image image;
    // Start is called before the first frame update
    [ClientCallback]
    void Start()
    {

        button.onClick.AddListener(SpawnBuilding);
    }

    [Client]
    public void SpawnBuilding()
    {
        NetworkClient.localPlayer.GetComponent<ClientPlayer>().CmdSpawnBuilding(buildingSettings.buildingName, GetMousePos());
    }

    [Client]
    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;


        //Minus the size of the screen from the mouse position
        mousePos = new Vector3(Screen.width, Screen.height, 0) - mousePos;

        mousePos.z = Camera.main.gameObject.transform.position.z;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePosition;
    }

}
