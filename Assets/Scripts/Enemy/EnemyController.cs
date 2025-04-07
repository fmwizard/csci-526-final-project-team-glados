using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;


public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundCheckRadius = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpPressed;
    private float currentVelocityMagnitude;
    private SpriteRenderer spriteRenderer;
    //public bool fromPortal;
    public event Action OnDeathOrDisable;
    

    private void OnDisable(){
        OnDeathOrDisable?.Invoke();
    }
    private void OnDestroy(){
        OnDeathOrDisable?.Invoke();
    }
    private void Awake()
    {
        this.enabled = false;
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
        

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPressed = true;
        }

        // OLD CODE FOR IJL CONTROLS
        //horizontalInput = 0;
        // if (Input.GetKey(KeyCode.J))
        // {
        //     horizontalInput = -1;
        // }
        // else if (Input.GetKey(KeyCode.L))
        // {
        //     horizontalInput = 1;
        // }

        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     jumpPressed = true;
        // }

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
        if (collision.gameObject.CompareTag("Hostility"))
        {
            float damage = 9999f;
            // Try to apply damage to enemy
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (FirebaseManager.instance != null)
                {
                    Vector2 pos = transform.position;
                    int level = PlayerStats.levelNumber;
                    FirebaseManager.instance.LogEnemyKill("Ally", pos, level);
                }
                GetComponent<Enemy>().TakeDamage(damage);
            }
        }

        // Check to see if the player is riding to make it stick
        // if(collision.gameObject.CompareTag("Player"))
        // {
        //     //Vector2 pointOfContact = collision.contacts[0].point;
        //     float allyTop = GetComponent<Collider2D>().bounds.max.y;
        //     float playerBottom = collision.collider.bounds.min.y;

        //     if(playerBottom >= allyTop - 0.1f)
        //     {
        //         collision.transform.SetParent(transform);
        //     }
        // }
        
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // //Vector2 pointOfContact = collision.contacts[0].point;
            // float allyTop = GetComponent<Collider2D>().bounds.max.y;
            // float playerBottom = collision.collider.bounds.min.y;

            // if(playerBottom >= allyTop - 0.1f)
            // {
            //     collision.transform.SetParent(transform);
            //     // Vector2 enemyPosition = transform.position;
            //     // Vector2 playerOffset = new Vector2(0, allyTop - playerBottom);
            //     // collision.transform.position = enemyPosition + playerOffset;


            // }
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>();
            if(playerCollider != null)
            {
                PhysicsMaterial2D extremeStickyMaterial = new PhysicsMaterial2D("ExtremeStickyMaterial");
                extremeStickyMaterial.friction = 100f;
                playerCollider.sharedMaterial = extremeStickyMaterial;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // if(collision.gameObject.CompareTag("Player") && collision.transform.parent == transform)
        // {
        //     collision.transform.SetParent(null);
        // }

        if(collision.gameObject.CompareTag("Player"))
        {
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>();
            if(playerCollider != null)
            {
                PhysicsMaterial2D activeMaterial = new PhysicsMaterial2D("ActiveMaterial");
                activeMaterial.friction = 0f;
                playerCollider.sharedMaterial = activeMaterial;
            }
        }
    }

}