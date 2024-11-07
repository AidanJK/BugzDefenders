using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private AudioSource movementAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movementAudioSource = GetComponent<AudioSource>(); // Get the AudioSource component

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
        // Get input for movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize movement to prevent faster diagonal movement
        if (movement.sqrMagnitude > 1)
        {
            movement = movement.normalized;
        }

        // Flip the sprite based on direction
        spriteRenderer.flipX = !facingRight();

        // Set animator parameters
        animator.SetBool("isRunning", movement != Vector2.zero);

        // Play or stop the movement sound based on player movement
        if (IsMoving() && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
        else if(!IsMoving())
        {
            movementAudioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is with the Tilemap tagged as "Tiles"
        if (collision.gameObject.CompareTag("Tiles"))
        {
            Debug.Log("Collided with Tiles");
            // Add any specific collision handling logic here if necessary
        }
    }

    // Method to check if the player is moving
    public bool IsMoving()
    {
        return movement != Vector2.zero;
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
        return mousePosition.x >= transform.position.x;
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

