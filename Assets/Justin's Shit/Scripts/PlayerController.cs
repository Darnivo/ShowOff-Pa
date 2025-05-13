using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;  // how far below the collider bottom we check

    private Rigidbody rb;
    private CapsuleCollider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        MoveHorizontal();
        if (Input.GetButtonDown("Jump") && IsGrounded())
            Jump();
    }

    private void MoveHorizontal()
    {
        float h = Input.GetAxis("Horizontal");              // A/D or ←/→
        Vector3 vel = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);
        rb.linearVelocity = vel;
    }

    private void Jump()
    {
        // zero out any downward velocity before jump
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        // position of the sphere: at the bottom of the capsule, slightly inset
        Vector3 spherePos = transform.position
            + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);

        // cast a small sphere to check for ground
        return Physics.CheckSphere(
            spherePos,
            col.radius - 0.02f,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    // visualize the ground-check in editor
    void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<CapsuleCollider>();
        Vector3 spherePos = transform.position
            + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spherePos, col.radius - 0.02f);
    }
}
