using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectilePrefab;    // Reference to the projectile prefab
    public Transform firePointRight;       // Fire point when facing right
    public Transform firePointLeft;        // Fire point when facing left
    private Transform firePointCurrent;    // Current fire point based on facing direction

    public float attackRate = 2f;          // Number of attacks per second
    public float shootAnimationLength = 1.2f; // Total length of the shoot animation in seconds

    public float stationaryDelayOffset = 0.5f; // Delay offset when stationary
    public float movingDelayOffset = 1.2f;     // Delay offset when moving

    private float nextAttackTime = 0f;     // Time until the next attack is allowed
    private Animator animator;             // Reference to the Animator component

    // Reference to the player's movement script
    private PlayerMovement playerMovement;

    // Variable to store the shooting direction
    private Vector2 shootDirection;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("Animator component missing from the player.");
        if (firePointRight == null)
            Debug.LogError("FirePointRight reference is missing.");
        if (firePointLeft == null)
            Debug.LogError("FirePointLeft reference is missing.");
        if (projectilePrefab == null)
            Debug.LogError("Projectile prefab is not assigned.");

        // Get the PlayerMovement component
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("PlayerMovement script missing from the player.");
    }

    void Update()
    {
        if (playerMovement.IsMoving())
        {
            if (playerMovement.facingRight())
                firePointCurrent = firePointRight;
            else
                firePointCurrent = firePointLeft;
        }
        else
        {
            if (playerMovement.facingRight())
                firePointCurrent = firePointRight;
            else
                firePointCurrent = firePointLeft;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button click
            {
                // Calculate shooting direction
                CalculateShootDirection();

                // Trigger the Shoot animation
                animator.SetTrigger("Shoot");

                // Determine if the player is moving
                bool isMoving = playerMovement.IsMoving();

                // Determine the delay based on movement
                float delayOffset = isMoving ? movingDelayOffset : stationaryDelayOffset;

                // Start the coroutine to instantiate the projectile after a delay
                StartCoroutine(InstantiateProjectileAfterDelay(shootAnimationLength - delayOffset, firePointCurrent));

                // Update the next attack time
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    // Method to calculate and store the shooting direction
    void CalculateShootDirection()
    {
        // Get mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Ensure z is zero in 2D space

        // Calculate direction from firePoint to mouse position
        shootDirection = (mousePosition - firePointCurrent.position).normalized;

        // Debug: Log the direction
        Debug.Log("Calculated shoot direction: " + shootDirection);
    }

    // Coroutine to instantiate the projectile after a specified delay
    IEnumerator InstantiateProjectileAfterDelay(float delay, Transform firePointCurrent)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePointCurrent.position, Quaternion.identity);

        // Rotate the projectile to face the shooting direction
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Set the projectile's direction
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(shootDirection);
        }
        else
        {
            Debug.LogError("Projectile script not found on the projectile prefab.");
        }

        Debug.Log("Projectile instantiated after delay of " + delay + " seconds.");
    }
}




