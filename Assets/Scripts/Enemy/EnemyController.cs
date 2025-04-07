using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckRadius = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private int initHitCount = 2;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private float currentVelocityMagnitude;
    private SpriteRenderer spriteRenderer;
    //public bool fromPortal;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!groundCheck)
        {
            groundCheck = transform;
        }
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Update()
    {
        horizontalInput = 0;
        if (Input.GetKey(KeyCode.J))
        {
            horizontalInput = -1;
        }
        else if (Input.GetKey(KeyCode.L))
        {
            horizontalInput = 1;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            jumpPressed = true;
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Flip sprite based on movement direction
        if (horizontalInput != 0)
        {
            spriteRenderer.flipX = horizontalInput < 0;
        }
        
        currentVelocityMagnitude = rb.velocity.magnitude;
    }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
    //     {
    //         // Calculate damage based on velocity
    //         float damage = currentVelocityMagnitude;
    //         // Try to apply damage to enemy
    //         Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    //         if (enemy != null)
    //         {
    //             enemy.TakeDamage(damage);
    //         }
    //     }
    // }        // Check if we hit an enemy

    private void FixedUpdate()
    {
        // Handle movement
        Vector2 moveVelocity = rb.velocity;

        moveVelocity.x = horizontalInput * moveSpeed;

        // Handle jumping
        if (jumpPressed && isGrounded)
        {
            moveVelocity.y = jumpForce;
            jumpPressed = false;
        }
        else if (!isGrounded)
        {
            // Prevent jumping in mid-air
            jumpPressed = false;
        }

        rb.velocity = moveVelocity;
    }

    public float GetCurrentVelocityMagnitude()
    {
        return currentVelocityMagnitude;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float damage = 999f;
        if (collision.gameObject.CompareTag("Hostility"))
        {
            // Try to apply damage to enemy
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, collision.gameObject);
                GetComponent<Enemy>().TakeDamage(damage, collision.gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("RedEnemy"))
        {
            RedEnemy redEnemy = collision.gameObject.GetComponent<RedEnemy>();
            if (redEnemy != null)
            {
                redEnemy.SetHitCount(initHitCount - 1);
                redEnemy.TakeDamage(damage, collision.gameObject);
                GetComponent<Enemy>().TakeDamage(damage, collision.gameObject);
            }
        }
    }

}