using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour, IDeathHandler
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpVelocity = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Rope Settings")]
    public float attachRange = 2f;
    public LayerMask ropeLayer;
    public float swingForce = 50f;
    public float swingJumpVelocity = 8f;
    public float releaseBoost = 8f;
    public float climbSpeed = 1.5f;
    public float ropeSegmentLength = 1f;

    [Header("Ice Settings")]
    public LayerMask iceMask;
    public float iceAcceleration = 20f;
    public float maxIceSpeed = 8f;

    [Header("Wall-Jump Settings")]
    public LayerMask stickyWallMask;
    public float wallJumpHorizontal = 6f;
    public float wallJumpVertical = 8f;
    public float wallSlideSpeed = 0f;

    private bool touchingWall = false;
    private Vector3 wallNormal;
    private bool wallSliding = false;

    [Header("Ground Settings")]
    public LayerMask groundMask;
    public float groundCheckOffset = 0.1f;

    [Header("Respawn Settings")]
    public Transform respawnPoint;

    private Rigidbody rb;
    private CapsuleCollider col;
    private bool isSwinging;
    private HingeJoint swingJoint;
    private List<Rigidbody> ropeSegments;
    private float totalRopeLength;
    private float climbDistance;
    private Transform ropeRoot;
    private Collider[] ropeCols;
    private float attachCooldown;
    private Collider[] playerCols;
    private bool gotKey;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        playerCols = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (attachCooldown > 0f)
            attachCooldown -= Time.deltaTime;

        if (isSwinging &&
            (swingJoint == null ||
             swingJoint.connectedBody == null ||
             !swingJoint.connectedBody.gameObject.activeInHierarchy))
        {
            ForceDetach();
            return;
        }

        Vector3 groundCheckPos = transform.position +
            Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
        bool grounded = Physics.CheckSphere(
            groundCheckPos,
            col.radius - 0.02f,
            groundMask | iceMask,
            QueryTriggerInteraction.Ignore
        );
        bool onIceSurface = Physics.CheckSphere(
            groundCheckPos,
            col.radius - 0.02f,
            iceMask,
            QueryTriggerInteraction.Ignore
        );

        if (isSwinging)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rb.AddForce(Vector3.right * h * swingForce * Time.deltaTime, ForceMode.VelocityChange);

            float v = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(v) > 0.01f)
                climbDistance = Mathf.Clamp(
                    climbDistance - v * climbSpeed * Time.deltaTime,
                    0f,
                    totalRopeLength
                );
            UpdateJointAnchor();

            if (Input.GetButtonDown("Jump"))
                DetachAndLaunch();
        }
        else
        {
            if (attachCooldown <= 0f && TryAttachRope())
                return;

            float h = Input.GetAxisRaw("Horizontal");
            wallSliding = touchingWall
              && !grounded
              && Mathf.Abs(wallNormal.x) > 0.1f;

            if (!wallSliding)
            {
                if (grounded)
                {
                    if (onIceSurface)
                    {
                        if (Mathf.Abs(h) > 0.01f)
                        {
                            rb.AddForce(
                                Vector3.right * h * iceAcceleration * Time.deltaTime,
                                ForceMode.VelocityChange
                            );
                            float clamped = Mathf.Clamp(
                                rb.linearVelocity.x,
                                -maxIceSpeed,
                                maxIceSpeed
                            );
                            rb.linearVelocity = new Vector3(clamped, rb.linearVelocity.y, 0f);
                        }
                    }
                    else
                    {
                        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);
                    }
                }
                else
                {
                    if (Mathf.Abs(h) > 0.01f)
                        rb.linearVelocity = new Vector3(h * moveSpeed, rb.linearVelocity.y, 0f);
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (grounded)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, 0f);
                }
                else if (wallSliding)
                {
                    float dir = -Mathf.Sign(wallNormal.x);
                    rb.linearVelocity = new Vector3(
                        dir * wallJumpHorizontal,
                        wallJumpVertical,
                        0f
                    );
                    wallSliding = false;
                    touchingWall = false;
                }

            }
        }
    }

    void FixedUpdate()
    {
        if (!isSwinging)
        {
            if (wallSliding)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z
                );
            }
            else if (rb.linearVelocity.y < 0f)
            {
                rb.linearVelocity += Vector3.up
                    * Physics.gravity.y
                    * (fallMultiplier - 1f)
                    * Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
            {
                rb.linearVelocity += Vector3.up
                    * Physics.gravity.y
                    * (lowJumpMultiplier - 1f)
                    * Time.fixedDeltaTime;
            }
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if ((stickyWallMask.value & (1 << col.gameObject.layer)) == 0)
            return;

        if (rb.linearVelocity.y > 0.1f)
        {
            touchingWall = false;
            return;
        }

        foreach (var cp in col.contacts)
        {
            if (Mathf.Abs(cp.normal.y) < 0.2f)
            {
                touchingWall = true;
                wallNormal = cp.normal;
                return;
            }
        }

        touchingWall = false;
    }


    private void OnCollisionExit(Collision col)
    {
        if ((stickyWallMask.value & (1 << col.gameObject.layer)) != 0)
            touchingWall = false;
    }

    private void ForceDetach()
    {
        if (ropeCols != null)
            foreach (var rc in ropeCols)
                foreach (var pc in playerCols)
                    Physics.IgnoreCollision(rc, pc, false);
        if (swingJoint != null) Destroy(swingJoint);
        isSwinging = false;
    }
    private bool TryAttachRope()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attachRange, ropeLayer);
        if (hits.Length == 0) return false;

        Collider nearest = hits.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First();
        Rigidbody firstRb = nearest.attachedRigidbody;
        if (firstRb == null) return false;

        ropeRoot = firstRb.transform.parent;
        ropeSegments = ropeRoot.GetComponentsInChildren<Rigidbody>()
            .Where(r => (ropeLayer.value & (1 << r.gameObject.layer)) != 0)
            .OrderByDescending(r => r.transform.position.y)
            .ToList();
        totalRopeLength = ropeSegments.Count * ropeSegmentLength;

        Vector3 worldHit = nearest.ClosestPoint(transform.position);
        int idx = ropeSegments.IndexOf(firstRb);
        float segY = ropeSegments[idx].transform.position.y;
        float nextY = idx < ropeSegments.Count - 1
            ? ropeSegments[idx + 1].transform.position.y
            : segY - ropeSegmentLength;
        float frac = Mathf.InverseLerp(segY, nextY, worldHit.y);
        climbDistance = idx * ropeSegmentLength + frac * ropeSegmentLength;

        swingJoint = gameObject.AddComponent<HingeJoint>();
        swingJoint.connectedBody = firstRb;
        swingJoint.autoConfigureConnectedAnchor = false;
        swingJoint.anchor = new Vector3(0f, col.height / 2f, 0f);
        swingJoint.axis = Vector3.forward;
        swingJoint.useLimits = true;
        swingJoint.limits = new JointLimits { min = -90f, max = 90f };
        swingJoint.enableCollision = false;

        ropeCols = ropeRoot.GetComponentsInChildren<Collider>();
        Collider[] playerCols = GetComponentsInChildren<Collider>();
        foreach (var rc in ropeCols)
            foreach (var pc in playerCols)
                Physics.IgnoreCollision(rc, pc, true);

        isSwinging = true;
        UpdateJointAnchor();
        return true;
    }

    private void UpdateJointAnchor()
    {
        climbDistance = Mathf.Clamp(climbDistance, 0f, totalRopeLength);
        float exactPos = climbDistance / ropeSegmentLength;
        int idx = Mathf.Clamp(Mathf.FloorToInt(exactPos), 0, ropeSegments.Count - 1);
        float frac = Mathf.Clamp01(exactPos - idx);
        int next = Mathf.Min(idx + 1, ropeSegments.Count - 1);
        Vector3 a = ropeSegments[idx].transform.position;
        Vector3 b = ropeSegments[next].transform.position;
        Vector3 worldA = Vector3.Lerp(a, b, frac);
        swingJoint.connectedBody = ropeSegments[idx];
        swingJoint.connectedAnchor = ropeSegments[idx].transform.InverseTransformPoint(worldA);
    }

    private void DetachAndLaunch()
    {
        Collider[] playerCols = GetComponentsInChildren<Collider>();
        if (ropeCols != null)
            foreach (var rc in ropeCols)
                foreach (var pc in playerCols)
                    Physics.IgnoreCollision(rc, pc, false);

        Vector3 anchorWS = swingJoint.connectedBody.transform.TransformPoint(swingJoint.connectedAnchor);
        Vector3 dir = (transform.position - anchorWS).normalized;
        Vector3 tangent = new Vector3(-dir.y, dir.x, 0f).normalized;
        if (rb.linearVelocity.x < 0f) tangent = -tangent;
        Destroy(swingJoint);
        isSwinging = false;
        rb.linearVelocity = tangent * releaseBoost + Vector3.up * swingJumpVelocity;
        attachCooldown = 0.5f;
    }

    public void onDeath()
    {
        transform.position = new Vector3(respawnPoint.position.x, respawnPoint.position.y + 2, transform.position.z);
        transform.rotation = respawnPoint.rotation;
        rb.linearVelocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isSwinging ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachRange);
        if (col != null)
        {
            Vector3 sp = transform.position + Vector3.down * (col.height / 2f - col.radius + groundCheckOffset);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(sp, col.radius - 0.02f);
        }
    }
    public void OnKeyCollected()
    {
        if (gotKey == false) gotKey = true;

    }
    public void OnKeyLost()
    {
        if (gotKey == true) gotKey = false;
    }

}
