using UnityEngine;

public class PendulumSwing : MonoBehaviour
{
    private HingeJoint hinge;
    private JointMotor motor;
    public float force = 100;
    public float velocity = 50;

    public Transform attachPoint; // Optional: you can set a specific child transform as attach point
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // player.AttachToPendulum(rb);
            }
        }
    }
    
    



}