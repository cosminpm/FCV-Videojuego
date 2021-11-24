using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed, dashDistance;
    private Vector3 _lastMoveDir;
    public AudioSource dashSound;
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
            Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
            TryMove(moveDir, speed * Time.deltaTime);
        }
    }

    private bool CanMove(Vector3 dir, float distance)
    {
        return  !Physics.Raycast(transform.position, dir, distance);
    }

    private bool TryMove(Vector3 baseMoveDir, float distance)
    {
        Vector3 moveDir = baseMoveDir;
        bool canMove = CanMove(moveDir, distance);
        if (!canMove)
        {
            // Can't move diagonally
            moveDir = new Vector3(baseMoveDir.x, 0f,0f).normalized;
            canMove = moveDir.x != 0 && CanMove(moveDir, distance);
            if (!canMove)
            {
                // Cannot move horizontally
                moveDir = new Vector3(0f, 0f,baseMoveDir.z).normalized;
                canMove = moveDir.z != 0f && CanMove(moveDir, distance);
            }
        }
        if (canMove)
        {
            _lastMoveDir = moveDir;
            transform.position += moveDir * distance;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!dashSound.isPlaying)
            {
                dashSound.Play();
            }
            transform.position += _lastMoveDir * dashDistance;
        }
    }
}