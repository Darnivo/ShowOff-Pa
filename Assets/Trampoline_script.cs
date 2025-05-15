using UnityEngine;

public class Trampoline_script : MonoBehaviour
{
    public float bounceForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.attachedRigidbody;
        if (rb != null)
        {
            // Reset vertical velocity first (optional, for consistency)
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;

            // Apply upward force
            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);
        }
    }
}
