using UnityEngine;

public class PlatformStickiness : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object has a Rigidbody and is not kinematic
        if (collision.rigidbody != null && !collision.rigidbody.isKinematic)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null && collision.transform.parent == transform)
        {
            collision.transform.SetParent(null);
        }
    }
}
