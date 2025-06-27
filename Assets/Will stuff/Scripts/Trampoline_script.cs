using UnityEngine;

public class Trampoline_script : MonoBehaviour
{
    public float bounceForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.attachedRigidbody;
        if (rb != null)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayTrampolineSound();
            }
        }
    }
}