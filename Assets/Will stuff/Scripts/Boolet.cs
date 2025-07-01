using UnityEngine;

public class Boolet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.onDeath();
            }
            Destroy(gameObject);
        }   
        

       else if (other.CompareTag("Sight"))
        {
            Debug.Log("Hit Sight object - destroying bullet");
            Destroy(gameObject);
        }
    }
}