using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFollowPlayer : MonoBehaviour
{

    public Transform player;             // Reference to the player's transform
    public float followSpeed = 2f;       // Speed at which the bug follows the player
    public float stopDistance = 1.5f;    // Distance at which the bug stops following the player
    public float bufferDistance = 1f;    // Additional buffer to prevent oscillation

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFollowing = false;
    private Transform currentTarget;     // The current target, either the player or the enemy

    public int maxHealth = 20;
    private int currentHealth;

    public float detectionRadius = 3.0f;     // Radius within which the bug detects enemies
    public float attackRange = 1.0f;         // Range within which the bug can attack the enemy
    public int damageAmount = 5;             // Damage dealt to the enemy
    public float attackCooldown = 1.5f;      // Cooldown time between attacks

    private float lastAttackTime = 0f;       // Tracks time since last attack
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        currentTarget = player; // Start with the player as the target
    }

    void FixedUpdate()
    {
        if (rb == null || currentTarget == null) return;

        // Check if an enemy is within detection range
        UpdateTarget();

        // Follow or attack based on current target
        if (currentTarget.CompareTag("Enemy"))
        {
            float distanceToEnemy = Vector2.Distance(transform.position, currentTarget.position);
            if (distanceToEnemy <= attackRange)
            {
                AttackEnemy();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
        else
        {
            FollowPlayer();
        }
    }

    void UpdateTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform nearestEnemy = null;
        float nearestEnemyDistance = detectionRadius;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < nearestEnemyDistance)
                {
                    nearestEnemyDistance = distance;
                    nearestEnemy = hit.transform;
                }
            }
        }

        // Set the current target: prioritize nearest enemy if within detection radius, otherwise the player
        currentTarget = nearestEnemy != null ? nearestEnemy : player;
    }

    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance + bufferDistance)
        {
            isFollowing = true;
        }
        else if (distanceToPlayer <= stopDistance)
        {
            isFollowing = false;
            rb.velocity = Vector2.zero; // Stop movement when within stop distance
        }

        if (isFollowing)
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.velocity = direction * followSpeed;

        // Flip sprite based on movement direction
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void AttackEnemy()
    {
        // Trigger the attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            EnemyUnit enemy = currentTarget.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
                Debug.Log("Bug attacked the enemy!");
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    public void Die()
    {
        // Trigger the death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Disable the bug’s collider and stop it from moving
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // Destroy the bug after the animation finishes
        Destroy(gameObject, 1f); // Adjust the delay as needed
    }

}

