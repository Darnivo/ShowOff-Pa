using UnityEngine;

public class Fan : MonoBehaviour
{
    public float pushForce = 10f;
    public Vector3 pushDirection = Vector3.forward; // Default direction

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize force direction in the editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.rotation * pushDirection.normalized * 2f);
    }
}
