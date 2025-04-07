using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PortalGunCollectible : MonoBehaviour
{
    public GameObject player;
    public PopupManager popupManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))// && SceneManager.GetActiveScene().name == "tutorial")
        {
            LineRenderer line = other.GetComponent<LineRenderer>();
            if(line != null){
                line.enabled = true;
            }

            if(popupManager != null)
            {
                popupManager.ShowPopupText();
            }

            Destroy(gameObject);
        }
    }


}
