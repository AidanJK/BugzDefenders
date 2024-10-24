using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Transform[] patrolPoints; // Array of points between which the enemy will patrol
    public float patrolSpeed = 2f; // Speed at which the enemy patrols
    public float stopDistance = 1.5f; // Distance at which the enemy stops near the player
    public Transform player; // Reference to the player's transform
    public float detectionRadius = 5f; // Radius within which the enemy will detect and chase the player
    private int currentPointIndex = 0; // Current patrol point index
    private bool chasingPlayer = false; // Flag to indicate if the enemy is chasing the player
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public int damageAmount = 10; // Amount of damage the enemy deals to the player

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component attached to this GameObject
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                chasingPlayer = true; // Start chasing the player
            }
            else if (distanceToPlayer > detectionRadius)
            {
                chasingPlayer = false; // Return to patrolling
            }

            if (chasingPlayer && distanceToPlayer > stopDistance)
            {
                // Move towards the player
                Vector3 directionToPlayer = player.position - transform.position;
                transform.position += directionToPlayer.normalized * patrolSpeed * Time.fixedDeltaTime;

                // Flip the sprite based on movement direction
                if (directionToPlayer.x > 0)
                {
                    spriteRenderer.flipX = true; // Face left
                }
                else if (directionToPlayer.x < 0)
                {
                    spriteRenderer.flipX = false; // Face right
                }
            }
            else if (!chasingPlayer)
            {
                Patrol(); // Patrol between the points
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return; // Exit if no patrol points are set

        // Get the target patrol point
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 directionToPoint = targetPoint.position - transform.position;

        // Move towards the patrol point
        transform.position += directionToPoint.normalized * patrolSpeed * Time.fixedDeltaTime;

        // Flip the sprite based on movement direction
        if (directionToPoint.x > 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
        else if (directionToPoint.x < 0)
        {
            spriteRenderer.flipX = false; // Face right
        }

        // Check if reached the patrol point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
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
                Debug.Log("Player collided with enemy");
                playerMovement.TakeDamage(damageAmount); // Apply damage to the player on collision
            }
        }
    }
}
