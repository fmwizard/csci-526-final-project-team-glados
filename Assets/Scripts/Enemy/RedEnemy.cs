using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedEnemy : Enemy
{
    // Count the number of times the player hits
    private int hitCount = 0;

    // Define colors for hit states
    private Color firstColorAfterHit = new Color(0.3f, 0f, 0.5f, 1f);
    private Color secondColorAfterHit = new Color(0.6f, 0.3f, 1f, 1f);
    
    protected override void Update() {
        base.Update();
    }
    
    public override void TakeDamage(float damage) {
        // Check if damage is coming from a Box
        if (damage >= 9999f) 
        {
            Debug.Log("RedEnemy was hit by a Box! Instantly dying.");
            Die(); 
            return;
        }
        // Increase the hit counter every time the player hits the enemy
        hitCount++;

        StartCoroutine(DamageFlash());

        if (hitCount == 1) {
            spriteRenderer.color = firstColorAfterHit;
        }
        else if (hitCount == 2) {
            spriteRenderer.color = secondColorAfterHit;
        }
        else if (hitCount >= 3) {
            if (FirebaseManager.instance != null)
            {
                Vector2 pos = transform.position;
                int level = PlayerStats.levelNumber;
                FirebaseManager.instance.LogEnemyKill("Box", pos, level);
            }
            Die();
            return;
        }
        StartCoroutine(DamageFlash());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the player collided with the HeadTrigger
            HeadTrigger headTrigger = GetComponentInChildren<HeadTrigger>();

            if (headTrigger != null && collision.otherCollider == headTrigger.GetComponent<Collider2D>())
            {
                Debug.Log("Player touched RedEnemy's head - it gets hit.");
                // Do not respawn if the player lands on the head
                return; 
            }

            Debug.Log("Player touched RedEnemy's body! Respawn");
        }
    }
}
