using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PortalGunCollectible : MonoBehaviour
{
    public GameObject player;
    public PopupManager popupManager;

    private PortalManager portalManager;

    void Start()
    {
        portalManager = FindObjectOfType<PortalManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LineRenderer line = other.GetComponent<LineRenderer>();
            if (SceneManager.GetActiveScene().name == "tutorial")
            {
                portalManager.CanUsePortal = true;
                if (line != null)
                {
                    line.enabled = true;
                }
            }
            else if (SceneManager.GetActiveScene().name == "allyTutorial")
            {
                portalManager.CanUseCage = true;
            }

            if (popupManager != null)
            {
                popupManager.ShowPopupText();
            }

            Destroy(gameObject);
        }
    }


}
