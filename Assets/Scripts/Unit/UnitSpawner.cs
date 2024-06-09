using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform spawnPoint = null;

    public Button button;

    [Client]
    public void Awake()
    {
        button.onClick.AddListener(SpawnUnit);
    }
    public void SpawnUnit()
    {

        CmdSpawnUnit();

    }

    [Command(requiresAuthority = false)]
    private void CmdSpawnUnit(NetworkConnectionToClient connectionToClient = null)
    {
        GameObject unitInstance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);



        Debug.Log("Spanwed unit ");
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked on unit spawner.");
        SpawnUnit();
    }
}