using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpVelocity = 7f;

    [Header("Gravity Modifiers")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Swing Settings")]
    public float attachRange = 2f;
    public LayerMask ropeLayer;
    public float swingForce = 50f;
    public float swingJumpVelocity = 8f;
    [Tooltip("How hard you’re flung along the tangent when you detach")]
    public float releaseBoost = 8f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    private bool isSwinging = false;
    private HingeJoint swingJoint;
    private Collider[] playerCols;
    private Collider[] ropeCols;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ
                       | RigidbodyConstraints.FreezeRotation;
        playerCols = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (isSwinging)
        {
            float h = Input.GetAxis("Horizontal");
            rb.AddForce(Vector3.right * h * swingForce * Time.deltaTime, ForceMode.VelocityChange);

            if (Input.GetButtonDown("Jump"))
                DetachAndLaunch();
        }
        else
        {
            float h = Input.GetAxis("Horizontal");
            bool grounded = IsGrounded();

            if (grounded || Mathf.Abs(h) > 0.01f)
                rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);

            if (Input.GetButtonDown("Jump"))
            {
                if (TryAttachRope()) return;
                if (grounded) NormalJump();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isSwinging)
            ApplyGravityModifiers();
    }

    bool TryAttachRope()
    {
        var hits = Physics.OverlapSphere(transform.position, attachRange, ropeLayer);
        if (hits.Length == 0) return false;

        var nearest = hits.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First();
        var ropeRb = nearest.attachedRigidbody;
        if (!ropeRb) return false;

        swingJoint = gameObject.AddComponent<HingeJoint>();
        swingJoint.connectedBody = ropeRb;
        swingJoint.autoConfigureConnectedAnchor = false;
        swingJoint.anchor = new Vector3(0, col.height / 2f, 0);

        Vector3 worldHit = nearest.ClosestPoint(transform.position);
        swingJoint.connectedAnchor = ropeRb.transform.InverseTransformPoint(worldHit);

        swingJoint.axis = Vector3.forward;
        swingJoint.useLimits = true;
        swingJoint.limits = new JointLimits { min = -90, max = 90 };
        swingJoint.enableCollision = false;

        ropeCols = ropeRb.GetComponentsInChildren<Collider>();
        foreach (var rc in ropeCols)
            foreach (var pc in playerCols)
                Physics.IgnoreCollision(rc, pc, true);

        isSwinging = true;
        return true;
    }

    void DetachAndLaunch()
    {
        // Re-enable collisions
        if (ropeCols != null)
            foreach (var rc in ropeCols)
                foreach (var pc in playerCols)
                    Physics.IgnoreCollision(rc, pc, false);

        // Compute world-space anchor
        Vector3 anchorWS = swingJoint.connectedBody
            .transform
            .TransformPoint(swingJoint.connectedAnchor);

        // Direction from anchor to player
        Vector3 dir = (transform.position - anchorWS).normalized;

        // Base tangent (perp to rope) in XY-plane
        Vector3 tangent = new Vector3(-dir.y, dir.x, 0f).normalized;

        // If our horizontal velocity was negative, flip tangent to left
        if (rb.linearVelocity.x < 0f)
            tangent = -tangent;

        Destroy(swingJoint);
        isSwinging = false;

        // Launch: tangent boost + vertical boost
        rb.linearVelocity = tangent * releaseBoost
                    + Vector3.up * swingJumpVelocity;
    }

    void NormalJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, 0f);
    }

    void ApplyGravityModifiers()
    {
        if (rb.linearVelocity.y < 0f)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
    }

    bool IsGrounded()
    {
        Vector3 sp = transform.position
                   + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        return Physics.CheckSphere(sp, col.radius - 0.02f, groundMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isSwinging ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachRange);

        if (col)
        {
            Vector3 sp = transform.position
                       + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(sp, col.radius - 0.02f);
        }
    }
}
