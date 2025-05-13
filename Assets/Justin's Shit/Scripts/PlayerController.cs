using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public LayerMask groundMask;        // assign the "Ground" layer here
    public float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        HandleMovement();
        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");  // A/D or Left/Right
        float z = Input.GetAxis("Vertical");    // W/S or Up/Down if you want forward
        Vector3 move = transform.right * h + transform.forward * z;
        Vector3 vel = move * moveSpeed;
        vel.y = rb.linearVelocity.y;  // preserve vertical velocity
        rb.linearVelocity = vel;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        // cast a short ray down to check for ground
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayLength = col.bounds.extents.y + groundCheckDistance;
        return Physics.Raycast(origin, Vector3.down, rayLength, groundMask);
    }
}
