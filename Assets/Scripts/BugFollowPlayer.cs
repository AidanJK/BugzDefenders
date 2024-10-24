using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFollowPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float followSpeed = 2f; // Speed at which the bug follows the player
    public float stopDistance = 1.5f; // Distance at which the bug stops following the player

    void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate the distance between the bug and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // If the distance is greater than the stop distance, move towards the player
            if (distanceToPlayer > stopDistance)
            {
                // Calculate direction vector from the bug's position to the player's position
                Vector3 direction = player.position - transform.position;
                // Normalize the direction to get a consistent speed, then move the bug towards the player
                transform.position += direction.normalized * followSpeed * Time.fixedDeltaTime;
            }
        }
    }
}


