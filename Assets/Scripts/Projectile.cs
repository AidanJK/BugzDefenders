using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;         // Speed of the projectile
    public int damage = 20;           // Damage dealt by the projectile
    private Rigidbody2D rb;           // Reference to Rigidbody2D
    private Animator animator;        // Reference to Animator component
    private bool hasCollided = false; // Flag to prevent multiple collisions
    [SerializeField] private float range = 10.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Rigidbody2D component missing from the projectile.");
        if (animator == null)
            Debug.LogError("Animator component missing from the projectile.");
    }

    void Start()
    {

    }

    public void SetDirection(Vector2 direction)
    {
        // Set the projectile's velocity
        rb.velocity = direction.normalized * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only proceed if the collided object has the "Enemy" tag
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Proceed with impact logic
            if (hasCollided) return;
            hasCollided = true;

            // Stop the projectile movement
            rb.velocity = Vector2.zero;

            // Disable the collider to prevent further collisions
            GetComponent<Collider2D>().enabled = false;

            // Play the impact animation
            animator.SetTrigger("Impact");

            // Apply damage to the enemy
            EnemyUnit enemy = collision.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destroy the projectile after the impact animation
            float impactAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, impactAnimationLength);
        }
        if (collision.gameObject.CompareTag("Tiles"))
        {
            // Proceed with impact logic
            if (hasCollided) return;
            hasCollided = true;

            // Stop the projectile movement
            rb.velocity = Vector2.zero;

            // Disable the collider to prevent further collisions
            GetComponent<Collider2D>().enabled = false;

            // Play the impact animation
            animator.SetTrigger("Impact");

            // Destroy the projectile after the impact animation
            float impactAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, impactAnimationLength);
        }
    }
}


