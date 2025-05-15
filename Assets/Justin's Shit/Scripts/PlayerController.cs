using System.Linq;              // for .OrderBy()
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpVelocity = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Swing Settings")]
    public float attachRange = 2f;          // how close to rope you must be
    public LayerMask ropeLayer;             // set to “Rope” layer
    public float swingForce = 50f;          // how strongly A/D pushes you
    public float swingJumpVelocity = 8f;    // upward boost when detaching

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    private Rigidbody rb;
    private CapsuleCollider col;

    // swing state
    private bool isSwinging = false;
    private HingeJoint swingJoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // freeze Z so we stay in the X–Y plane
        rb.constraints = RigidbodyConstraints.FreezePositionZ 
                       | RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        if (isSwinging)
        {
            // swing input: A/D applies lateral force
            float h = Input.GetAxis("Horizontal");
            rb.AddForce(Vector3.right * h * swingForce * Time.deltaTime, ForceMode.VelocityChange);

            // press Space to detach and jump off
            if (Input.GetButtonDown("Jump"))
                DetachAndJump();
        }
        else
        {
            // normal side-to-side movement
            float h = Input.GetAxis("Horizontal");
            rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);

            // press Space
            if (Input.GetButtonDown("Jump"))
            {
                if (TryAttachRope())      return;  // if we attached, skip jump
                if (IsGrounded())         NormalJump();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isSwinging)
            ApplyJumpGravityModifiers();
        // if swinging, physics handles the rest
    }

    bool TryAttachRope()
    {
        // look for the nearest rope segment
        Collider[] hits = Physics.OverlapSphere(transform.position, attachRange, ropeLayer);
        if (hits.Length == 0) return false;

        // pick closest segment
        Collider nearest = hits.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First();
        Rigidbody ropeRb = nearest.attachedRigidbody;
        if (ropeRb == null) return false;

        // add a hinge joint from player to rope
        swingJoint = gameObject.AddComponent<HingeJoint>();
        swingJoint.connectedBody = ropeRb;
        swingJoint.autoConfigureConnectedAnchor = false;
        swingJoint.anchor = new Vector3(0f, col.height / 2f, 0f);
        // anchor on the rope segment at its local hit point
        Vector3 worldHit = nearest.ClosestPoint(transform.position);
        swingJoint.connectedAnchor = ropeRb.transform.InverseTransformPoint(worldHit);
        swingJoint.axis = Vector3.forward;
        swingJoint.useLimits = true;
        JointLimits lim = swingJoint.limits;
        lim.min = -90f; lim.max = 90f;
        swingJoint.limits = lim;
        swingJoint.enableCollision = false;

        isSwinging = true;
        rb.linearVelocity = Vector3.zero;  // reset any leftover velocity
        return true;
    }

    void DetachAndJump()
    {
        Destroy(swingJoint);
        isSwinging = false;
        // give a little upward boost
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, swingJumpVelocity, 0f);
    }

    void NormalJump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, 0f);
    }

    void ApplyJumpGravityModifiers()
    {
        if (rb.linearVelocity.y < 0f)
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
    }

    bool IsGrounded()
    {
        Vector3 spherePos = transform.position 
            + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        return Physics.CheckSphere(spherePos, col.radius - 0.02f, groundMask, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        // visualize attach range
        Gizmos.color = isSwinging ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachRange);

        // visualize ground-check
        if (col != null)
        {
            Vector3 spherePos = transform.position 
                + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spherePos, col.radius - 0.02f);
        }
    }
}
