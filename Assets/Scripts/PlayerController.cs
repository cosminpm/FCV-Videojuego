using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed, dashDistance, dashCooldown, rotationSpeed;
    private Vector3 _lastMoveDir;
    public AudioSource dashSound, axeFrontSound, axeCircularSound;
    private float DASH_MAX;
    public Animator axeAnimator;

    public Transform attackPointCircle, attackPointForward;
    public float attackRangeCircular,attackRangeForward, forcePushEnemies;
    public LayerMask enemyLayers;


    private void Update()
    {
        HandleDash();
        HandleMovement();
        HandleAttack();
    }

    private void Start()
    {
        DASH_MAX = 1f;
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
            //transform.forward = -moveDir;

            Quaternion toRotation = Quaternion.LookRotation(-moveDir);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandleAttack()
    {
        bool circularAxe = Input.GetMouseButtonDown(1);
        bool frontAxe = Input.GetMouseButtonDown(0);
        axeAnimator.SetBool("FrontAttack", frontAxe);
        axeAnimator.SetBool("CircularAttack", circularAxe);
        
        if (circularAxe)
        {
            // Sound
            if (!axeCircularSound.isPlaying && !axeFrontSound.isPlaying)
            {
                axeCircularSound.Play();
            }
            // Force to move enemies
            Collider[] hitEnemies = Physics.OverlapSphere(attackPointCircle.position, attackRangeCircular, enemyLayers);
            foreach (var enemy in hitEnemies)
            {
                Debug.Log("We hit C" + enemy.name);
                enemy.attachedRigidbody.AddForce(-enemy.transform.forward  * forcePushEnemies);
            }
        }

        if (frontAxe)
        {
            // Sound
            if (!axeFrontSound.isPlaying && !axeCircularSound.isPlaying)
            {
                axeFrontSound.Play();
            }
            // Force to move enemies
            Collider[] hitEnemies = Physics.OverlapBox(attackPointForward.position,
                new Vector3(attackRangeForward / 2, attackRangeForward / 2,attackRangeForward/2), new Quaternion(),enemyLayers);
            foreach (var enemy in hitEnemies)
            {
                Debug.Log("We hit F" + enemy.name);
                enemy.attachedRigidbody.AddForce(-enemy.transform.forward  * forcePushEnemies);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPointCircle == null)
            return;
        Gizmos.DrawWireSphere(attackPointCircle.position, attackRangeCircular);
        Gizmos.DrawCube(attackPointForward.position,new Vector3(attackRangeForward,attackRangeForward,attackRangeForward) );
    }


    private void HandleDash()
    {
        dashCooldown -= Time.deltaTime;
        if (dashCooldown < 0)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (TryMove(_lastMoveDir, dashDistance))
                {
                    // Dash correctly distance complete
                }
            
                else
                {
                    // Dash stop on wall
                    var position = transform.position;
                    RaycastHit hit;
                    Ray ray = new Ray(position, _lastMoveDir);
                    Physics.Raycast(ray, out hit);
                    transform.position += _lastMoveDir * hit.distance;
                }
            
                dashCooldown = DASH_MAX;
                if (!dashSound.isPlaying)
                {
                    dashSound.Play();
                }
                
               
            }
        }
        
        if (dashCooldown > 0)
        {
        }
    }
}