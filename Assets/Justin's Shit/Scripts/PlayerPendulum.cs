using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPendulum : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && transform.parent != null)
        {
            transform.SetParent(null);
            rb.isKinematic = false; 
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pendulum"))
        {
            transform.SetParent(other.transform);
            rb.isKinematic = true;
        }
    }
}