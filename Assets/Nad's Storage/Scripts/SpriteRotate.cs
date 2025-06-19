using UnityEngine;

public class SpriteRotate : MonoBehaviour
{
    public Transform visual; // The visual part that should lean (sprite or model)
    public float leanAngle = 10f; // Max tilt angle
    public float leanSpeed = 5f; // Smoothing speed
    public float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private Vector3 targetEuler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


}
