using System.Linq;
using Mirror;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{

    public static BuildingPreview Instance { get; private set; }
    public LayerMask buildableLayer;


    public GameObject previewBuilding;

    private string buildingName;

    [ClientCallback]
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }



    [Client]
    public void SetBuilding(string buildingName)
    {
        this.buildingName = buildingName;
        Building_Settings buildingData = GameCore.LoadBuildingData().FirstOrDefault(x => x.buildingName == buildingName);

        if (buildingData != null)
        {
            previewBuilding = Instantiate(buildingData.prefab);
            Building buildingScript = previewBuilding.GetComponent<Building>();
            buildingScript.isPreview = true;
        }
        else
        {
            //NOTE: This error file location should be dynamic
            Debug.LogError($"Building {buildingName} not found. It should be at Building/Building_Settings/{buildingName}");

        }
    }
    [ClientCallback]
    public void Update()
    {
        if (previewBuilding != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildableLayer))
            {
                previewBuilding.transform.position = hit.point;
            }

            if (Input.GetMouseButtonDown(0))
            {
                ClientPlayer player = NetworkClient.localPlayer.GetComponent<ClientPlayer>();
                player.ClientSpawnBuilding(previewBuilding.transform.position, previewBuilding.transform.rotation, buildingName);
                //If not holding shift remove the preview building
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    RemoveBuilding();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                RemoveBuilding();
            }
        }
    }
    [Client]
    public void RemoveBuilding()
    {
        Destroy(previewBuilding);
    }


}