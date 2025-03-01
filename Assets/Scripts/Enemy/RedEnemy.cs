using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemy : Enemy
{
    // Count the number of times the player hits
    private int hitCount = 0;

    protected override void Update() {
        base.Update();
    }
    
    public override void TakeDamage(float damage) {
        // Increase the hit counter every time the player hits the enemy
        hitCount++;

        StartCoroutine(DamageFlash());

        if (hitCount >= 3) {
            Die();
        }
    }

   private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("RedEnemy touched the player!");
            PlayerRespawn player = collision.gameObject.GetComponent<PlayerRespawn>();

            if (player != null)
            {
                player.Respawn(); 
            }
            else
            {
                Debug.LogError("PlayerRespawn script not found on Player!");
            }
        }
    }
}
