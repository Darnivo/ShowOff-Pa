using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;
    private bool goingToB = true;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            goingToB ? pointB : pointA, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, goingToB ? pointB : pointA) < 0.01f)
        {
            goingToB = !goingToB;
        }
    }

    

}

