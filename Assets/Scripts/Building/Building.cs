using UnityEngine;
using Mirror;

public class Building : Health
{
    public bool isPreview = false;

    [ServerCallback]
    private void Update()
    {
        if (!isPreview)
        {
            Tick(Time.deltaTime);
        }
    }

    [Server]
    public virtual void Tick(float deltaTime)
    {
        return;
    }
}