using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintZone : MonoBehaviour
{
    public GameObject hint; 
    public float timeToShowHint = 5f;

    private float timeInside = 0f;
    private bool hintShown = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timeInside += Time.deltaTime;

            if (timeInside >= timeToShowHint && !hintShown)
            {
                hint.SetActive(true);
                hintShown = true;
            }

            // Check if player moved past the area (to the right)
            if (other.transform.position.x > transform.position.x + GetComponent<BoxCollider2D>().bounds.extents.x)
            {
                hint.SetActive(false);
                timeInside = 0f;
                hintShown = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hint.SetActive(false);
            timeInside = 0f;
            hintShown = false;
        }
    }
}
