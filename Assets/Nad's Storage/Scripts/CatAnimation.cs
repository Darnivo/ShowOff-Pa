using UnityEngine;

public class CatAnimation : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void catMovement(float speed)
    {
        animator.SetFloat("xvelocity", speed);
    }

}