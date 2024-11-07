using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followSpeed = 2f;
    public float stopDistance = 1.5f;
    public float bufferDistance = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isFollowing = false;

    [Header("Health Settings")]
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Attack Settings")]
    public float attackRange = 1.0f;         // Range within which the bug can attack the enemy
    public int damageAmount = 5;             // Damage dealt to the enemy
    public float attackCooldown = 1.5f;      // Cooldown time between attacks

    private float lastAttackTime = 0f;       // Time since the last attack

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stopDistance + bufferDistance)
        {
            isFollowing = true;
        }
        else if (distanceToPlayer <= stopDistance)
        {
            isFollowing = false;
            rb.velocity = Vector2.zero;
        }

        if (isFollowing)
        {
            FollowPlayer();
        }

        // Check if an enemy is within range to attack
        CheckForEnemyAndAttack();
    }

    void FollowPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * followSpeed;

        // Flip sprite based on direction
        if (direction.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void CheckForEnemyAndAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))  // Assuming enemies have the tag "Enemy"
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    AttackEnemy(hit.GetComponent<EnemyUnit>());
                    lastAttackTime = Time.time; // Reset cooldown timer
                }
            }
        }
    }

    void AttackEnemy(EnemyUnit enemy)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damageAmount);
            Debug.Log("Bug attacked the enemy!");
        }
    }

    // Method to handle bug taking damage
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

    void Die()
    {
        Debug.Log(gameObject.name + " has died.");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject, 1f);
    }

}
