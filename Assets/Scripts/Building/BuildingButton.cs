using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
public class BuildingButton : MonoBehaviour
{
    public Building_Settings building;

    public Image buildIcon;

    public TMP_Text costText;


    private void OnValidate()
    {
        if (building != null)
        {
            buildIcon.sprite = building.icon;
            costText.text = building.cost.ToString();
        }

    }


    // Start is called before the first frame update
    [ClientCallback]
    void Start()
    {
        if (building != null)
        {
            buildIcon.sprite = building.icon;
            costText.text = building.cost.ToString();
            if (gameObject.TryGetComponent<Button>(out Button button))
            {
                button.onClick.AddListener(OnClick);
            }
            else
            {
                Debug.LogError($"No button found on {gameObject.name}");

            }
        }
    }

    [Client]
    public void OnClick()
    {
        if (building != null)
        {
            Debug.Log($"Clicked on {building.buildingName}");
            BuildingPreview.Instance.SetBuilding(building.buildingName);
        }
    }
}