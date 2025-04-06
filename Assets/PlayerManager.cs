using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //public GameObject player;
    public PlayerController player;
    private EnemyController currentEnemy;

    private bool controllingPlayer = true;

    void Start()
    {
        //playerController = playerObject.GetComponent<PlayerController>();
        player.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ToggleControl();
        }
    }

    public void SetCurrentEnemy(EnemyController newEnemy)
    {
        if (currentEnemy != null)
        {
            currentEnemy.OnDeathOrDisable -= OnEnemyLost;
        }

        currentEnemy = newEnemy;

        if (currentEnemy != null)
        {
            currentEnemy.OnDeathOrDisable += OnEnemyLost;

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
                player.enabled = false;
                currentEnemy.enabled = true;
                controllingPlayer = false;
            }
        }
        else
        {
            if (player != null)
            {
                if (currentEnemy != null)
                    currentEnemy.enabled = false;

                player.enabled = true;
                controllingPlayer = true;
            }
        }
    }
    // [SerializeField] private GameObject playerObject;
    // [SerializeField] private GameObject enemyObject;
    // private PlayerController playerController;
    // private EnemyController enemyController;

    // private bool controllingPlayer = true;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     playerController = playerObject.GetComponent<PlayerController>();
    //     enemyController = enemyObject.GetComponent<EnemyController>();
    //     if(playerController == null || enemyController == null){
    //         Debug.LogError("Player or enemy controller isn't assigned");
    //         return;
    //     }

    //     EnablePlayerControl();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.LeftShift))
    //     {
    //         //controllingPlayer = !controllingPlayer;
    //         if(!controllingPlayer){ // Enemy is not live therefore don't switch
    //             controllingPlayer = true;
    //             EnablePlayerControl();
    //         }
    //         else if (enemyObject != null && enemyObject.activeInHierarchy)
    //         {
    //             controllingPlayer = false;
    //             EnableEnemyControl();
    //         }
    //         else{
    //             Debug.Log("Enemy unavailable, player remains in control");
    //         }
    //     }
    // }

    // void EnablePlayerControl()
    // {
    //     playerController.enabled = true;
    //     enemyController.enabled = false;
    // }

    // void EnableEnemyControl()
    // {
    //     playerController.enabled = false;
    //     enemyController.enabled = true;
    // }

}
