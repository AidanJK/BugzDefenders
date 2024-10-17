using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 movement;

    void Update()
    {
        // Get input from WASD keys
        movement.x = 0f;
        movement.y = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            movement.y = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1f;
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }
}
