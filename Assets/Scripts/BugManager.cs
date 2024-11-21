using System.Collections;
using UnityEngine;

public class BugFollowPlayer : MonoBehaviour
{
    // Health Management
    public int maxHealth = 100;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public int strengthLevel = 1;
    public Transform player;
    public float followSpeed = 2f;
    public float stopDistance = 1.5f;
    public float detectionRadius = 5.0f;
    public bool isRanged = false;
    public GameObject projectilePrefab;
    public float shootCooldown = 2f;
    public int damageAmount = 10;
    public float attackRange = 1.0f;

    private Rigidbody2D rb;
    private Transform currentTarget;
    private float lastShootTime = 0f;
    private float lastAttackTime = 0f;
    public float attackCooldown = 1.5f;

    //private AudioSource attackAudioSource;
    //private AudioSource deathAudio;
    //private AudioSource hurttAudioSource;
    //private AudioSource shootProjectileClip;
    //public AudioClip attackEnemy;
    //public AudioClip getHurt;
    //public AudioClip deathClip;
    //public AudioClip shootClip;

    void Start()
    {
        //shootProjectileClip = gameObject.AddComponent<AudioSource>();
        //deathAudio = gameObject.AddComponent<AudioSource>();
        //attackAudioSource = gameObject.AddComponent<AudioSource>();
       // hurttAudioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentTarget = player;
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (rb == null || currentTarget == null) return;

        UpdateTarget();

        // For ranged bugs
        if (isRanged)
        {
            FollowPlayer(); // Only follow the player
            if (currentTarget != player && Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
            {
                TryShootProjectile(); // Shoot if an enemy is within range
            }
        }
        else
        {
            // For melee bugs, move towards and attack the enemy if in range
            if (currentTarget != player)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, currentTarget.position);
                if (distanceToEnemy > stopDistance) // Chase enemy if out of range
                {
                    MoveTowards(currentTarget.position);
                }
                else if (distanceToEnemy <= attackRange) // Attack if within range
                {
                    AttackEnemy();
                }
            }
            else
            {
                FollowPlayer(); // Otherwise, follow the player
            }
        }

        // Update animator's running parameter based on movement
        animator.SetBool("IsRunning", rb.velocity.sqrMagnitude > 0.01f);
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

        // Set current target for ranged bugs to the nearest enemy for shooting, but they still follow the player
        if (nearestEnemy != null)
        {
            currentTarget = nearestEnemy; // Target enemy for attacking
        }
        else
        {
            currentTarget = player; // Otherwise, set the player as the target for movement
        }
    }

    void FollowPlayer()
    {
        // Only move towards the player if currentTarget is the player
        if (currentTarget == player)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                MoveTowards(player.position);
            }
            else
            {
                rb.velocity = Vector2.zero; // Stop when within stopDistance
            }
        }
    }

    void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * followSpeed;
        spriteRenderer.flipX = direction.x > 0;
    }

    void TryShootProjectile()
    {
        if (Time.time - lastShootTime >= shootCooldown)
        {
            Vector2 directionToEnemy = (currentTarget.position - transform.position).normalized;
            //shootProjectileClip.PlayOneShot(shootClip);

            // Instantiate the projectile and set its direction and damage
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetDirection(directionToEnemy);
                projectileScript.SetDamage(damageAmount);
            }

            lastShootTime = Time.time; // Update last shoot time
        }
    }

    private void Die()
    {
        //deathAudio.PlayOneShot(deathClip);
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        Destroy(gameObject, 1f);
    }

    void AttackEnemy()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            EnemyUnit enemy = currentTarget.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                //attackAudioSource.PlayOneShot(attackEnemy);
                enemy.TakeDamage(damageAmount);
                Debug.Log("Bug attacked the enemy!");
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        //hurttAudioSource.PlayOneShot(getHurt);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
        else if (spriteRenderer != null)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
}
