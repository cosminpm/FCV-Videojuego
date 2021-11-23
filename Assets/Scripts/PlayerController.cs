using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed, dashDistance;
    private Vector3 _lastMoveDir;

    private void Update()
    {
        HandleMovement();
        HandleDash();
    }

    private void HandleMovement()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveZ = 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }

        bool isIdle = moveX == 0 && moveZ == 0;
        if (!isIdle)
        {
            // Normalize to not move faster diagonal
            Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
            _lastMoveDir = moveDir;
            transform.position += moveDir * speed * Time.deltaTime; 
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position += _lastMoveDir * dashDistance;
        }
    }
}