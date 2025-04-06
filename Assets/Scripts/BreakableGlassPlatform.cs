using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableGlassPlatform : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 2.5f; // Time before breaking
    [SerializeField] private float blinkInterval = 0.1f; // Blink speed

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool isBreaking = false;
    private bool isBroken = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Awake()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box")) && !isBreaking && !isBroken)
        {
            StartCoroutine(BlinkAndBreak());
        }
    }

    IEnumerator BlinkAndBreak()
    {
        isBreaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        spriteRenderer.enabled = false;
        platformCollider.enabled = false;
        isBroken = true;
        isBreaking = false;
    }

    public void ResetPlatform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        isBreaking = false;
        isBroken = false;
    }
}