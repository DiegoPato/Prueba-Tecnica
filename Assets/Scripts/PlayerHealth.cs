using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int health;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] PlayerMovement player;

    void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            player.KillPlayer();
            health = maxHealth;
        }
    }
}
