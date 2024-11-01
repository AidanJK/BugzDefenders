using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BugFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;             // Reference to the player's transform
    public float followSpeed = 2f;       // Speed at which the bug follows the player
    public float stopDistance = 1.5f;    // Distance at which the bug stops following the player
    public float bufferDistance = 1f;    // Additional buffer to prevent oscillation

    private Rigidbody2D rb;              // Reference to the Rigidbody2D component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    // Flags to manage movement states
    private bool isFollowing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from the bug.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing from the bug.");
        }

        if (player == null)
        {
            Debug.LogError("Player Transform reference is missing.");
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // Calculate the distance between the bug and the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Determine whether to follow or stop based on distance and buffer
        if (distanceToPlayer > stopDistance + bufferDistance)
        {
            isFollowing = true;
        }
        else if (distanceToPlayer <= stopDistance)
        {
            isFollowing = false;
            rb.velocity = Vector2.zero; // Stop movement when within stop distance
        }

        // Move towards the player if following
        if (isFollowing)
        {
            FollowPlayer();
        }
    }

    void FollowPlayer()
    {
        // Calculate direction vector from the bug's position to the player's position
        Vector2 direction = (player.position - transform.position).normalized;

        // Move the bug towards the player using Rigidbody2D velocity for smoother movement
        rb.velocity = direction * followSpeed;

        // Flip the sprite based on movement direction
        if (direction.x > 0)
        {
            // Moving Right
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            // Moving Left
            spriteRenderer.flipX = false;

        }
        // No flipping needed if direction.x == 0 (no horizontal movement)
    }
}
