using UnityEngine;

public class FrogAnimation : MonoBehaviour
{
    private Rigidbody rb;
    private Animator froge;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        froge = GetComponentInChildren<Animator>();
    }

    public void frogJumps(float jump)
    {
        froge.SetFloat("yVelocity", jump);
    }

    public void setIsJumping(bool isJumping)
    {
        froge.SetBool("isJumping", isJumping);
    }
    public void setGroundDistance(float distance)
    {
        froge.SetFloat("groundDistance", distance);
    }

}