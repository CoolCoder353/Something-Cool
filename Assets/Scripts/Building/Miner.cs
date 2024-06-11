using UnityEngine;
using Mirror;

public class Miner : Building
{
    public float generationSpeed = 1f;

    public float resourceGenerationAmount = 1f;

    private float timer = 0;


    [ServerCallback]
    public void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= generationSpeed)
        {
            Debug.Log($"Generating {resourceGenerationAmount} resources for player {connectionToClient}");
            GameCore.Instance.AddResourcesToPlayer(connectionToClient, resourceGenerationAmount);
            timer = 0;
        }
    }

}