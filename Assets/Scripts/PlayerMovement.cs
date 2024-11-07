using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int maxHealth = 100;
    private int currentHealth;
    public Slider healthSlider;
    public Image healthFillImage;

    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateHealthBarColor();
    }


    void Update()
    {
        // Get input from WASD keys or arrow keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize the movement vector to prevent faster diagonal movement
        if (movement.sqrMagnitude > 1)
        {
            movement = movement.normalized;
        }

        // Flip the sprite based on mouse position relative to player
        spriteRenderer.flipX = !facingRight();

        // Set the animator parameter based on movement
        animator.SetBool("isRunning", movement != Vector2.zero);
    }

    void FixedUpdate()
    {
        // Apply movement to the player character using Rigidbody2D for better physics handling
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Method to check if the player is moving
    public bool IsMoving()
    {
        if (movement != Vector2.zero)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Method to handle player taking damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        UpdateHealthBarColor();

        // Start the flash red coroutine for damage feedback
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // Change color to red
        yield return new WaitForSeconds(0.1f); // Short delay
        spriteRenderer.color = Color.white; // Revert color to normal
    }

    public bool facingRight()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool facingRight = mousePosition.x >= transform.position.x;
        return facingRight;
    }

    // Method to handle player death
    void Die()
    {
        // Play death animation
        animator.SetBool("isDead", true);

        // Disable player movement
        this.enabled = false; // Disable this script
        rb.velocity = Vector2.zero; // Stop the player movement

        // Trigger game over sequence in GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.PlayerDied();
        }
    }

    // Method to update the health bar color based on current health
    void UpdateHealthBarColor()
    {
        if (healthFillImage != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            healthFillImage.color = healthColor;
        }
    }
}
