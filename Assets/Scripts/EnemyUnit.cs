using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Transform[] patrolPoints;   // Array of points between which the enemy will patrol
    public float patrolSpeed = 2f;     // Speed at which the enemy patrols

    public Transform player;           // Reference to the player's transform
    public float detectionRadius = 5f; // Radius within which the enemy will detect and chase the player
    public float stopDistance = 1.5f;  // Distance at which the enemy stops near the player

    public int damageAmount = 10;      // Amount of damage the enemy deals to the player

    public int maxHealth = 50;         // Maximum health of the enemy
    private int currentHealth;         // Current health of the enemy

    private int currentPointIndex = 0; // Current patrol point index
    private bool chasingPlayer = false; // Flag to indicate if the enemy is chasing the player
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Rigidbody2D rb;            // Reference to the Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from the enemy.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing from the enemy.");
        }

        if (player == null)
        {
            Debug.LogError("Player Transform reference is missing.");
        }

        if (patrolPoints.Length == 0)
        {
            Debug.LogError("No patrol points assigned to the enemy.");
        }

        // Initialize health
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            chasingPlayer = true; // Start chasing the player
        }
        else
        {
            chasingPlayer = false; // Return to patrolling
        }

        if (chasingPlayer && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else if (!chasingPlayer)
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToPlayer * patrolSpeed * Time.fixedDeltaTime);

        // Flip the sprite based on movement direction
        if (directionToPlayer.x > 0)
        {
            // Moving Right
            spriteRenderer.flipX = true;
            
        }
        else if (directionToPlayer.x < 0)
        {
            // Moving Left
            spriteRenderer.flipX = false;
            
        }
        // No flipping needed if directionToPlayer.x == 0
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 directionToPoint = (targetPoint.position - transform.position).normalized;

        rb.MovePosition(rb.position + directionToPoint * patrolSpeed * Time.fixedDeltaTime);

        // Flip the sprite based on movement direction
        if (directionToPoint.x > 0)
        {
            // Moving Right
            spriteRenderer.flipX = true;
        }
        else if (directionToPoint.x < 0)
        {
            // Moving Left
            spriteRenderer.flipX = false;
        }
        // No flipping needed if directionToPoint.x == 0

        // Check if reached the patrol point
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Move to the next patrol point
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(damageAmount); // Apply damage to the player on collision
            }
        }
    }

    // Method to handle taking damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by damage amount

        // Optional: Visual feedback
        StartCoroutine(FlashRed());

        // Check if health has reached zero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle enemy death
    void Die()
    {

        // Disable enemy components
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // Destroy the enemy game object
        Destroy(gameObject);
    }

    // Coroutine for visual feedback when taking damage
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // Change color to red
        yield return new WaitForSeconds(0.1f); // Wait for a short time
        spriteRenderer.color = Color.white; // Revert color back to normal
    }
}
