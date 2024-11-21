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
    public AudioClip deathAudio;
    public AudioClip hurtAudio;
    private AudioSource movementAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource hurtAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movementAudioSource = GetComponent<AudioSource>();
        deathAudioSource = gameObject.AddComponent<AudioSource>();
        hurtAudioSource = gameObject.AddComponent<AudioSource>();
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
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.sqrMagnitude > 1)
        {
            movement = movement.normalized;
        }

        spriteRenderer.flipX = !facingRight();
        animator.SetBool("isRunning", movement != Vector2.zero);

        if (IsMoving() && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }
        else if (!IsMoving())
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
        if (collision.gameObject.CompareTag("Tiles"))
        {
            Debug.Log("Collided with Tiles");
        }
    }

    public bool IsMoving()
    {
        return movement != Vector2.zero;
    }

    public void TakeDamage(int damage)
    {
        hurtAudioSource.PlayOneShot(hurtAudio);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        UpdateHealthBarColor();
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

    public bool facingRight()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePosition.x >= transform.position.x;
    }

    void Die()
    {
        deathAudioSource.PlayOneShot(deathAudio);
        animator.SetBool("isDead", true);
        this.enabled = false;
        rb.velocity = Vector2.zero;

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.PlayerDied();
        }
    }

    void UpdateHealthBarColor()
    {
        if (healthFillImage != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            healthFillImage.color = healthColor;
        }
    }

    public void Respawn()
    {
        // Locate the spawn point by tag and set the player’s position
        GameObject spawnPointObj = GameObject.FindWithTag("SpawnPoint");
        if (spawnPointObj != null)
        {
            transform.position = spawnPointObj.transform.position;
        }
        else
        {
            Debug.LogWarning("Spawn point not found in the scene.");
        }

        currentHealth = maxHealth;
        animator.SetBool("isDead", false);
        this.enabled = true;
        UpdateHealthBarColor();
    }
}
