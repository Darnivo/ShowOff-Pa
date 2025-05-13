using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;          // initial impulse
    public float extraJumpForce = 10f;    // continuous “hold” force
    public float maxJumpTime = 0.5f;      // maximum seconds you can hold

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    // state for variable jump
    private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // 1) Horizontal movement
        float h = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);

        // 2) Start jump
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            // zero out downward velocity so jump always feels consistent
            Vector3 v = rb.linearVelocity; v.y = 0f; rb.linearVelocity = v;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 3) Hold jump to go higher
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                rb.AddForce(Vector3.up * extraJumpForce * Time.deltaTime, ForceMode.VelocityChange);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false; // reached max hold time
            }
        }

        // 4) Release jump early
        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        // cast a small sphere at the bottom of the capsule
        Vector3 spherePos = transform.position
            + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        return Physics.CheckSphere(
            spherePos,
            col.radius - 0.02f,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    // visualize the ground-check sphere in the editor
    void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<CapsuleCollider>();
        Vector3 spherePos = transform.position
            + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spherePos, col.radius - 0.02f);
    }
}
