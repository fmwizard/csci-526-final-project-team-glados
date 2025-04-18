using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintZone : MonoBehaviour
{
    public GameObject hint; 
    public float timeToShowHint = 5f;
    public Vector3 hintOffset = new Vector3(0, 3f, 0);
    public string hintText = "Enter hint text here";

    private float timeInside = 0f;
    private bool hintShown = false;
    private GameObject currentHint;
    private PlayerController playerController;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hintShown)
        {
            timeInside += Time.deltaTime;

            if (timeInside >= timeToShowHint)
            {
                //hint.SetActive(true);
                hintShown = true;
                playerController = other.GetComponent<PlayerController>();
                ShowHint(other.transform);
            }

            // ONLY FOR AUTO REMOVING HINT IF PLAYER PASSES HINTZONE
            // // Check if player moved past the area (to the right)
            // if (other.transform.position.x > transform.position.x + GetComponent<BoxCollider2D>().bounds.extents.x)
            // {
            //     hint.SetActive(false);
            //     timeInside = 0f;
            //     hintShown = false;
            // }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //hint.SetActive(false);
            // timeInside = 0f;
            // hintShown = false;
            ResetHint();
        }
    }

    // private void ShowHint(Transform player)
    // {
    //     Debug.Log("Attempting to show hint");
    //     currentHint = Instantiate(hint, player.position + hintOffset, Quaternion.identity, transform);
    //     // Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + hintOffset);
    //     // currentHint.GetComponent<RectTransform>().position = screenPos;


    //     if (currentHint != null)
    //         Debug.Log("Hint instantiated at: " + currentHint.transform.position);
    //     else
    //         Debug.LogWarning("Hint failed to instantiate!");

    //     // Disable player input until spacebar is pressed
    //     if(playerController != null)
    //     {
    //         playerController.enabled = false;
    //     }

    //     StartCoroutine(WaitForSpace());
    // }

    private void ShowHint(Transform player)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found!");
            return;
        }

        // Instantiate UI Hint
        currentHint = Instantiate(hint, canvas.transform);
        currentHint.SetActive(true); // enable hint since prefab is not active by default

        // Set text dynamically
        TextMeshProUGUI hintTextComponent = currentHint.GetComponentInChildren<TextMeshProUGUI>();
        if (hintTextComponent != null)
        {
            hintTextComponent.text = hintText;
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI found on hint prefab!");
        }

        // Convert world position to screen position for placement above player
        Vector3 worldPos = player.position + hintOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Apply screen position to RectTransform
        RectTransform hintRect = currentHint.GetComponent<RectTransform>();
        hintRect.position = screenPos;

        // Optionally adjust pivot, anchor if needed
        //hintRect.pivot = new Vector2(0.5f, 0f); // Bottom center of hint aligns above head

        // Disable player movement or input
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        StartCoroutine(WaitForSpace());
    }


    private IEnumerator WaitForSpace()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        if(playerController != null)
        {
            playerController.enabled = true;
        }

        if(currentHint != null)
        {
            Destroy(currentHint);
        }

        ResetHint();
    }

    private void ResetHint()
    {
        timeInside = 0f;
        hintShown = false;
    }
}
