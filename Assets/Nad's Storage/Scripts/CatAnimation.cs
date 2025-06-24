using System;
using UnityEngine;

public class CatAnimation : MonoBehaviour
{
    public Animator animator;
    [HideInInspector]
    public bool isJumping = false;
    [Header("Ground Check")]
    public Transform groundCheck; // Transform to check for ground
    public float maxCheckDistance = 1f; // Maximum distance to check for ground
    public LayerMask groundLayers; // Layer mask to define what is considered ground
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        UpdateGroundDistance();
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            setJumping(true);
        }

    }

    void FixedUpdate()
    {
        float xVelocity = Math.Abs(rb.linearVelocity.x);
        float yVelocity = rb.linearVelocity.y;
        catMovement(xVelocity);
        setYVelocity(yVelocity);
    }
    public void catMovement(float speed)
    {
        animator.SetFloat("xvelocity", speed);
    }

    public void setYVelocity(float yVelocity)
    {
        animator.SetFloat("yvelocity", yVelocity);
    }
    public void setJumping(bool jump)
    {
        isJumping = jump;
        animator.SetBool("isJumping", jump);
    }
    public void setGroundDistance(float distance)
    {
        animator.SetFloat("groundDistance", distance);
    }

    private void UpdateGroundDistance()
    {
        if (Physics.Raycast(groundCheck.position, Vector3.down, out RaycastHit hit, maxCheckDistance, groundLayers))
        {
            float distance = hit.distance;
            setGroundDistance(distance);
        }
        Debug.DrawRay(groundCheck.position, Vector3.down * maxCheckDistance, Color.red);
    }

    private void OnCollisionEnter(Collision collision)
    {
        setJumping(false);
    }


}