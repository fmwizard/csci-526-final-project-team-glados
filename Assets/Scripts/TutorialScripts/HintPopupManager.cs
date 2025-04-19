using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class HintPopupManager : MonoBehaviour
{
    public static HintPopupManager Instance;
    private Canvas popupCanvas;
    public GameObject hintPrefab;
    public Vector3 hintOffset = new Vector3(0, 3f, 0);

    private GameObject currentHint;
    private Coroutine hintCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Find popup canvas and assign
        GameObject canvasObject = GameObject.Find("PopupCanvas");
        if(canvasObject != null)
        {
            popupCanvas = canvasObject.GetComponent<Canvas>();
        }
        else
        {
            Debug.LogWarning("Popup Canvas not found in scene");
        }
    }

    public void ShowHint(Transform player, string hintText, Action onDismiss = null)
    {
        
        if (popupCanvas == null || hintPrefab == null)
        {
            Debug.LogWarning("Popup Canvas or hintPrefab not assigned");
            return;
        }

        // Check to see if there is an active hint already and destroy
        if(currentHint != null)
        {
            Destroy(currentHint);
            //currentHint = null;
        }
        
        // Same for coroutine
        if(hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            //currentHint = null;
        }

        // Instantiate UI Hint
        currentHint = Instantiate(hintPrefab, popupCanvas.transform);
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
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popupCanvas.GetComponent<RectTransform>(),
            Camera.main.WorldToScreenPoint(worldPos),
            Camera.main,
            out anchoredPos
        );

        // Apply to RectTransform
        RectTransform hintRect = currentHint.GetComponent<RectTransform>();
        hintRect.anchoredPosition = anchoredPos;

        hintCoroutine = StartCoroutine(HintFollowsPlayer(player, onDismiss));
    }

    public IEnumerator HintFollowsPlayer(Transform player, Action onDismiss)
    {
        RectTransform hintRect = currentHint.GetComponent<RectTransform>();
        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();

        while (currentHint != null)
        {
            // World position above player
            Vector3 worldPos = player.position + hintOffset;

            // Convert to canvas local point
            Vector2 anchoredPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Camera.main.WorldToScreenPoint(worldPos),
                Camera.main,
                out anchoredPos
            );

            hintRect.anchoredPosition = anchoredPos;

            // Wait until space is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Destroy(currentHint);
                //currentHint = null;
                //StopCoroutine(hintCoroutine);
                onDismiss?.Invoke(); // calls the input function if necessary
                yield break;
            }

            yield return null;
        }
    }
}