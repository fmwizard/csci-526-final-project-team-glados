using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponentInParent<EnemyController>() != null) return;
        if (collision.CompareTag("Player"))
        {
            float damage = collision.GetComponent<PlayerController>().GetCurrentVelocityMagnitude();
            if (FirebaseManager.instance != null)
            {
                Vector2 pos = transform.position;
                int level = PlayerStats.levelNumber;
                FirebaseManager.instance.LogEnemyKill("Player", pos, level);
            }
            transform.parent.GetComponent<Enemy>().TakeDamage(damage);

            Debug.Log("Player hit the enemy's head and did NOT die.");
        }
        // Kill RedEnemy instantly if hit by Box
        else if (collision.CompareTag("Box")) 
        {
            Enemy enemy = transform.parent.GetComponent<Enemy>();
            Rigidbody2D boxRb = collision.GetComponent<Rigidbody2D>();
            // Instantly kill RedEnemy

            if (boxRb != null)
            {
                float boxSpeed = boxRb.velocity.magnitude;
                float requiredSpeed = 7f;
                float normalspeed = 1f;

                Debug.Log("Box speed: " + boxSpeed);

                // Only kill RedEnemy if the Box is moving fast enough
                if (enemy is RedEnemy) 
                {
                    if (boxSpeed >= requiredSpeed) 
                    {
                        Debug.Log("RedEnemy hit on head by high-speed box! Instantly dying.");
                        if (FirebaseManager.instance != null)
                        {
                            Vector2 pos = transform.position;
                            int level = PlayerStats.levelNumber;
                            FirebaseManager.instance.LogEnemyKill("Acclerated Box", pos, level);
                        }
                        enemy.TakeDamage(9999f);
                    }
                    else if (boxSpeed >= normalspeed) {
                        Debug.Log("Box hit detected! counting as one hit");
                        enemy.TakeDamage(1);
                    }
                    else
                    {
                        Debug.Log("Box hit RedEnemy but was too slow!");
                    }
                }
                else
                {
                    if (FirebaseManager.instance != null)
                    {
                        Vector2 pos = transform.position;
                        int level = PlayerStats.levelNumber;
                        FirebaseManager.instance.LogEnemyKill("Box", pos, level);
                    }
                    enemy.TakeDamage(1f);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
