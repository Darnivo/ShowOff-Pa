using UnityEngine;

public class FrogAnimation : MonoBehaviour
{
    private Rigidbody rb;
    private Animator froge;
    public bool isJumping = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        froge = GetComponentInChildren<Animator>();
    }

    public void frogJumps(float jump)
    {
        froge.SetFloat("yVelocity", jump);
    }

    public void setIsJumping(bool jump)
    {
        isJumping = jump;
        froge.SetBool("isJumping", jump);
    }
    public void setGroundDistance(float distance)
    {
        froge.SetFloat("groundDistance", distance);
    }
    public void frogAwake()
    {
        froge.SetBool("isAwake", true);
    }



}