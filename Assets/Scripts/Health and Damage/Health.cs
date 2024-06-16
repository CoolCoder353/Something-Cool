using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
using NaughtyAttributes;


public class Health : NetworkBehaviour
{
    [ShowNonSerializedField]
    public float maxHealth = 100;
    [ShowNonSerializedField]
    private bool isDead = false;
    [ShowNonSerializedField]
    private bool destroyOnDead = true;

    [SyncVar(hook = nameof(OnHealthChanged)), ShowNonSerializedField]
    public float currentHealth;


    public Slider healthBar;

    [Client]
    private void OnHealthChanged(float oldHealth, float newHealth)
    {
        healthBar.value = newHealth / maxHealth;
    }

    [Server]
    private void Die()
    {
        isDead = true;
        if (destroyOnDead)
        {
            // Destroy the game object on the server -> destroys it on all clients
            NetworkServer.Destroy(gameObject);
        }
    }

    [Server]
    public void TakeDamage(float damage)
    {
        Debug.Log($"Took {damage} damage");
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth == 0)
        {
            Die();
        }
    }


    public bool IsDead()
    {
        return isDead;
    }
}