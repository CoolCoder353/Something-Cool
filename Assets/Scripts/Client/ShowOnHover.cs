using UnityEngine;
using Mirror;


public class ShowOnHover : MonoBehaviour
{
    public GameObject objectToToggle;

    [ClientCallback]
    private void Start()
    {
        objectToToggle.SetActive(false);
    }
    [ClientCallback]
    private void OnMouseEnter()
    {
        objectToToggle.SetActive(true);
    }
    [ClientCallback]
    private void OnMouseExit()
    {
        objectToToggle.SetActive(false);
    }
}