using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Transform[] patrolPoints; // Assigned dynamically by EnemySpawner
    public float patrolSpeed = 2f;

    public GameObject playerObject; // Reference to player GameObject, for accessing components
    public Transform playerPosition;   // Reference to player Transform, for tracking position
    public float detectionRadius = 5f;
    public float stopDistance = 0.5f;
    public bool invertedFlip = false;

    public int damageAmount = 10;
    public float attackCooldown = 1.0f;
    public int maxHealth = 50;
    private int currentHealth;

    public int strengthLevel = 1; // Define the strength level for the enemy
    public GameObject bugAllyPrefab; // Bug ally prefab associated with this enemy

    private int currentPointIndex = 0;
    private bool chasingPlayer = false;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private Transform currentTarget;
    private float lastAttackTime = 0f;

    // Delegate and event for enemy death
    public delegate void DeathHandler(GameObject enemy);
    public event DeathHandler OnDeath;

    private PlayerParty playerParty;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Find and assign the PlayerParty component on the player
        playerParty = playerObject.GetComponent<PlayerParty>();

        if (patrolPoints == null || patrolPoints.Length < 2)
        {
            Debug.LogError("Patrol points not assigned or insufficient patrol points assigned.");
        }
    }

    public void AssignBugAlly(List<GameObject> potentialBugAllies)
    {
        if (potentialBugAllies.Count > 0)
        {
            bugAllyPrefab = potentialBugAllies[Random.Range(0, potentialBugAllies.Count)];
        }
        else
        {
            Debug.LogWarning("No matching bug allies found for enemy strength level.");
        }
    }

    void FixedUpdate()
    {
        if (playerObject == null || rb == null) return;

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
        else if (chasingPlayer && distanceToTarget <= stopDistance + 0.1f)
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
        Transform nearestEnemy = null;
        float nearestEnemyDistance = detectionRadius;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Ally"))  // Assuming bugs have the tag "Ally"
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < nearestEnemyDistance)
                {
                    nearestEnemyDistance = distance;
                    nearestEnemy = hit.transform;
                }
            }
        }

        currentTarget = nearestEnemy != null ? nearestEnemy : playerPosition;
    }

    void ChaseTarget()
    {
        Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
        rb.MovePosition(rb.position + directionToTarget * patrolSpeed * Time.fixedDeltaTime);
        if (invertedFlip)
            spriteRenderer.flipX = directionToTarget.x < 0;
        else
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
            else if (currentTarget.CompareTag("Player"))
            {
                PlayerMovement playerScript = currentTarget.GetComponent<PlayerMovement>();
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
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 directionToPoint = (targetPoint.position - transform.position).normalized;

        rb.MovePosition(rb.position + directionToPoint * patrolSpeed * Time.fixedDeltaTime);

        if (directionToPoint.x > 0 && !invertedFlip)
        {
            spriteRenderer.flipX = true;
        }
        else if (directionToPoint.x < 0 && !invertedFlip)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionToPoint.x > 0 && invertedFlip)
        {
            spriteRenderer.flipX = false;
        }
        else if (directionToPoint.x < 0 && invertedFlip)
        {
            spriteRenderer.flipX = true;
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
            animator.SetTrigger("Die");
        }

        // Add bug to the player's party if the bugAllyPrefab is assigned
        if (playerParty != null && bugAllyPrefab != null)
        {
            Debug.Log("Adding bug to player party: " + bugAllyPrefab.name);
            playerParty.AddBugToParty(bugAllyPrefab);
        }
        else
        {
            Debug.LogWarning("PlayerParty or BugAllyPrefab is missing. Bug not added.");
        }

        // Trigger OnDeath event if there are listeners
        if (OnDeath != null)
        {
            OnDeath.Invoke(gameObject);
        }

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        Destroy(gameObject, 0.45f);
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
