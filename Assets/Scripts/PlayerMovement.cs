using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the player moves
    public int maxHealth = 100; // Maximum health of the player
    private int currentHealth; // Current health of the player

    public Slider healthSlider; // Reference to the health slider in the UI
    public Image healthFillImage; // Reference to the Image component of the health slider fill area

    private Vector2 movement; // Variable to store movement input
    private Animator animator; // Reference to the Animator component
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component attached to this GameObject
        animator = GetComponent<Animator>(); // Get the Animator component attached to this GameObject
        currentHealth = maxHealth; // Set current health to maximum health
        healthSlider.maxValue = maxHealth; // Set slider max value to match player max health
        healthSlider.value = currentHealth; // Initialize slider value
        UpdateHealthBarColor(); // Set initial health bar color
    }

    void Update()
    {
        // Get input from WASD keys
        movement.x = 0f;
        movement.y = 0f;

        // Check for each input key and set movement accordingly
        if (Input.GetKey(KeyCode.W))
        {
            movement.y = 1f; // Move up
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y = -1f; // Move down
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1f; // Move left
            spriteRenderer.flipX = true; // Flip the sprite horizontally to face left
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1f; // Move right
            spriteRenderer.flipX = false; // Reset the sprite flip to face right
        }

        // Set the animator parameter based on movement
        if (movement != Vector2.zero)
        {
            animator.SetBool("isRunning", true); // Set isRunning to true when there is movement
        }
        else
        {
            animator.SetBool("isRunning", false); // Set isRunning to false when idle
        }
    }

    void FixedUpdate()
    {
        // Apply movement to the player character
        // FixedUpdate is used for physics-related calculations for consistent results
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Method to handle player taking damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0 or above maxHealth
        healthSlider.value = currentHealth; // Update the health slider to reflect the new health value
        UpdateHealthBarColor(); // Update health bar color based on current health

        if (currentHealth <= 0)
        {
            // Handle player death, such as triggering a death animation or restarting the level
            Debug.Log("Player has died");
        }
    }

    // Method to update the health bar color based on current health
    void UpdateHealthBarColor()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
    }
}
