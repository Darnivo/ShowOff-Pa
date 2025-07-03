using UnityEngine;

public class Boolet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public LayerMask wallLayer = 8; // Set in inspector, or use LayerMask.NameToLayer("Wall")
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Bullet hit: " + other.gameObject.name + " on layer: " + other.gameObject.layer);

        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.onDeath();
            }
            Destroy(gameObject);
        }

        // Check if the object is on the wall layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Sticky Wall"))
        {
            // Debug.Log("Hit wall - destroying bullet");
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Sight"))
        {
            // Debug.Log("Hit wall - destroying bullet");
            Destroy(gameObject);
        }

    }
}