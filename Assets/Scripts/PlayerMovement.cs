using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;           // Speed at which the player moves
    public int maxHealth = 100;            // Maximum health of the player
    private int currentHealth;             // Current health of the player

    public Slider healthSlider;            // Reference to the health slider in the UI
    public Image healthFillImage;          // Reference to the Image component of the health slider fill area

    private Vector2 movement;              // Variable to store movement input
    private Animator animator;             // Reference to the Animator component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    private Rigidbody2D rb;                // Reference to the Rigidbody2D component

    void Start()
    {
        // Get components and check for null references
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from the player.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component missing from the player.");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from the player.");
        }

        // Initialize health
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        else
        {
            Debug.LogError("HealthSlider reference is missing.");
        }

        if (healthFillImage == null)
        {
            Debug.LogError("HealthFillImage reference is missing.");
        }

        UpdateHealthBarColor(); // Set initial health bar color
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
        currentHealth -= damage; // Reduce current health by damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health between 0 and maxHealth                                                         

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Update the health slider
        }

        UpdateHealthBarColor(); // Update health bar color based on current health

        if (currentHealth <= 0)
        {
            // Handle player death
            Debug.Log("Player has died");
            Die();
        }
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
            // Interpolate between red (low health) and green (full health)
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            healthFillImage.color = healthColor;
        }
    }
}
