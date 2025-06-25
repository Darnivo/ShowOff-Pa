using System;
using UnityEngine;

public class CatAnimation : MonoBehaviour
{
    public Animator animator;
    [HideInInspector]
    public bool isJumping = false;
    [Header("Ground Check")]
    // public Transform groundCheck; // Transform to check for ground
    public LayerMask groundLayers; // Layer mask to define what is considered ground
    
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
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
    public void isLanding(bool land) {
        animator.SetBool("isLanding", land);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping)
        {
            setJumping(false);
        }

    }


}