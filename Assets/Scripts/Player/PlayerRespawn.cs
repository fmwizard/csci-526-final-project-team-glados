using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private float fallThreshold = -10f;
    private Vector2 startPosition;
    

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            LogDeath("Fall");
            Respawn();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Collision detected with: " + other.gameObject.name);
        if (other.CompareTag("Trap")) 
        {
            LogDeath("Trap");
            Respawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Hostility") && collision.gameObject.layer != LayerMask.NameToLayer("Companion") )
        {
            if (collision.gameObject.GetComponent<HeadTrigger>() == null)
            {
                LogDeath("Enemy");
                Respawn();
            }
        }
    }

    void LogDeath(string reason)
    {
        if (FirebaseManager.instance != null)
        {
            Vector2 pos = transform.position;
            float time = Time.timeSinceLevelLoad;
            int level = PlayerStats.levelNumber;
            DeathReasonData deathData = new DeathReasonData(reason, pos, time);
            FirebaseManager.instance.LogTestDataByPOST("deathReasons", deathData, level);
        }
    }


    public void Respawn()
    {
        if (FirebaseManager.instance != null)
        {
            PlayerStats.IncreaseDeathCount();
            FirebaseManager.instance.UpdateDeathCount(PlayerStats.levelNumber);
        }

        transform.position = startPosition;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Remvoe portals upon respawn for anticheating
        PortalManager portalManager = FindObjectOfType<PortalManager>();
        if(portalManager != null)
        {
            portalManager.RemovePortals();
        }

        EnemyController enemyController = FindObjectOfType<EnemyController>(true);
        if(enemyController != null)
        {
            Enemy enemy = enemyController.GetComponent<Enemy>();
            enemy.gameObject.SetActive(true);
            enemy.TakeDamage(9999f);
        }

        MainCamera cameraScript = Camera.main.GetComponent<MainCamera>();
        if(cameraScript != null)
        {
            cameraScript.ResetPlayer(transform);
        }
    }
}