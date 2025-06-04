using UnityEngine;

public class pinball : MonoBehaviour
{
    public float bounceForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate bounce direction (away from wall)
                Vector3 bounceDir = (other.transform.position - transform.position).normalized;
                rb.linearVelocity = Vector3.zero; // optional: reset current velocity
                rb.AddForce(bounceDir * bounceForce, ForceMode.Impulse);
            }
        }
    }
}
