using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;         // Speed of the projectile
    public int damage = 20;           // Default damage of the projectile
    private Rigidbody2D rb;           // Reference to Rigidbody2D
    private Animator animator;        // Reference to Animator component
    private bool hasCollided = false; // Flag to prevent multiple collisions

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Rigidbody2D component missing from the projectile.");
        if (animator == null)
            Debug.LogError("Animator component missing from the projectile.");
    }

    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction.normalized * speed;
    }

    // New method to set damage from an external source
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hasCollided) return;
            hasCollided = true;

            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Impact");

            EnemyUnit enemy = collision.GetComponent<EnemyUnit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Use the updated damage
            }

            float impactAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, impactAnimationLength);
        }
        if (collision.gameObject.CompareTag("Tiles"))
        {
            if (hasCollided) return;
            hasCollided = true;

            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Impact");

            float impactAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, impactAnimationLength);
        }
    }
}



