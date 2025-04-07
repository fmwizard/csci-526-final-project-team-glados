using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    private EnemyController currentEnemy;

    public bool controllingPlayer = true;
    private PlayerController playerController;
    
    public PhysicsMaterial2D activeMaterial;
    public PhysicsMaterial2D inactiveMaterial;
    private Collider2D playerCollider;

    void Start()
    {
        // Get playercontroller script attached to player
        playerController = player.GetComponent<PlayerController>();
        playerCollider = player.GetComponent<Collider2D>();
        // Probably won't need: playerCollider.sharedMaterial = activeMaterial;

        playerController.enabled = true;
        // playerRenderer.material = activeMaterial;
    }

    void Update()
    {
        // Toggle control if left shift is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControl();
        }
    }

    public void SetCurrentEnemy(EnemyController newEnemy)
    {
        // Changes control of enemy
        if (currentEnemy != null)
        {
            currentEnemy.OnDeathOrDisable -= OnEnemyLost;
            currentEnemy.enabled = false;
        }

        currentEnemy = newEnemy;

        if (currentEnemy != null)
        {
            currentEnemy.OnDeathOrDisable += OnEnemyLost;
            currentEnemy.enabled = false;

            // If we were already controlling an enemy, switch back to player first
            if (!controllingPlayer)
            {
                ToggleControl(); // back to player
            }
        }
    }

    private void OnEnemyLost()
    {
        if (!controllingPlayer) // Only act if we were controlling the enemy
        {
            ToggleControl(); // Switch back to player
        }
    }

    private void ToggleControl()
    {
        if (controllingPlayer)
        {
            if (currentEnemy != null && currentEnemy.gameObject.activeInHierarchy)
            {
                playerController.enabled = false;
                playerCollider.sharedMaterial = inactiveMaterial;
                playerController.lineRenderer.enabled = false;
                currentEnemy.enabled = true;
                controllingPlayer = false;
            }
        }
        else
        {
            if (playerController != null)
            {
                if (currentEnemy != null)
                    currentEnemy.enabled = false;

                playerController.enabled = true;
                playerCollider.sharedMaterial = activeMaterial;
                playerController.lineRenderer.enabled = true;
                controllingPlayer = true;
            }
        }
        //Debug.Log($"Shift Toggle. controllingPlayer: {controllingPlayer}. currentEnemy: {currentEnemy}");
    }
}