using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;

    public Transform player;
    public float detectionRadius = 5f;
    public float stopDistance = 0.5f; // Adjusted for closer proximity to bugs

    public int damageAmount = 10;
    public float attackCooldown = 1.0f; // Time in seconds between attacks

    public int maxHealth = 50;
    private int currentHealth;

    private int currentPointIndex = 0;
    private bool chasingPlayer = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator; // Reference to Animator component

    private Transform currentTarget;
    private float lastAttackTime = 0f; // Tracks the last time an attack was made

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Get Animator component
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        UpdateTarget();

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        if (distanceToTarget <= detectionRadius)
        {
            chasingPlayer = true;
        }
        else
        {
            chasingPlayer = false;
        }

        if (chasingPlayer && distanceToTarget > stopDistance)
        {
            ChaseTarget();
        }
        else if (chasingPlayer && distanceToTarget <= stopDistance)
        {
            AttackTarget(); // Attack when close enough
        }
        else
        {
            Patrol();
        }
    }

    void UpdateTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Transform nearestBug = null;
        float nearestBugDistance = detectionRadius;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ally"))  // Assuming bugs have the tag "Ally"
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < nearestBugDistance)
                {
                    nearestBugDistance = distance;
                    nearestBug = hit.transform;
                }
            }
        }

        currentTarget = nearestBug != null ? nearestBug : player;
    }

    void ChaseTarget()
    {
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToTarget * patrolSpeed * Time.fixedDeltaTime);

        spriteRenderer.flipX = directionToTarget.x > 0;
    }

    void AttackTarget()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (currentTarget.CompareTag("Ally"))
            {
                BugFollowPlayer bugScript = currentTarget.GetComponent<BugFollowPlayer>();
                if (bugScript != null)
                {
                    bugScript.TakeDamage(damageAmount);
                    Debug.Log("Enemy damaged the bug!");
                }
            }
            else if (currentTarget == player)
            {
                PlayerMovement playerScript = player.GetComponent<PlayerMovement>();
                if (playerScript != null)
                {
                    playerScript.TakeDamage(damageAmount);
                    Debug.Log("Enemy damaged the player!");
                }
            }

            lastAttackTime = Time.time;
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 directionToPoint = (targetPoint.position - transform.position).normalized;

        rb.MovePosition(rb.position + directionToPoint * patrolSpeed * Time.fixedDeltaTime);

        if (directionToPoint.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (directionToPoint.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Trigger death animation
        }

        // Disable enemy behavior and collider
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // Destroy the game object after the animation completes
        Destroy(gameObject, 0.45f); // Adjust the delay to match animation length
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
