using UnityEngine;

public class SpriteRotate : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform visual; // The visual part that should lean (sprite or model)
    public float leanAngle = 15f; // Max tilt angle in degrees
    public float leanSpeed = 5f; // Smoothing speed
    public float groundCheckDistance = 1.5f; // Distance to check for ground
    public float raycastOffset = 0.5f; // Horizontal offset for edge detection
    
    [Header("Ground Detection")]
    public LayerMask groundLayer; // Set this to your ground layer
    public bool debugMode = false; // Show debug rays in scene view
    
    private Rigidbody rb;
    private float currentRotation = 0f;
    private PlayerController playerController;
    private CapsuleCollider playerCollider;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        playerCollider = GetComponent<CapsuleCollider>();
        
        // If visual is not assigned, try to find the sprite
        if (visual == null && transform.childCount > 0)
        {
            visual = transform.GetChild(0);
        }
        
        // If groundLayer is not set, try to use the groundMask from PlayerController
        if (groundLayer.value == 0 && playerController != null)
        {
            groundLayer = playerController.groundMask;
        }
    }
    
    void Update()
    {
        // Only apply rotation when grounded
        if (IsGrounded())
        {
            float targetRotation = CalculateGroundAngle();
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, leanSpeed * Time.deltaTime);
        }
        else
        {
            // Smoothly return to zero rotation when in air
            currentRotation = Mathf.Lerp(currentRotation, 0f, leanSpeed * Time.deltaTime);
        }
        
        // Apply rotation to visual
        if (visual != null)
        {
            visual.localRotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }
    
    bool IsGrounded()
    {
        // Check if player is grounded using a sphere cast similar to PlayerController
        float checkRadius = playerCollider != null ? playerCollider.radius - 0.02f : 0.48f;
        Vector3 groundCheckPos = transform.position + Vector3.down * (playerCollider.height / 2f - playerCollider.radius + 0.1f);
        
        return Physics.CheckSphere(groundCheckPos, checkRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }
    
    float CalculateGroundAngle()
    {
        // Cast two rays downward from left and right of the character
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastOffset;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastOffset;
        
        RaycastHit leftHit, rightHit;
        bool leftHitGround = Physics.Raycast(leftRayOrigin, Vector3.down, out leftHit, groundCheckDistance, groundLayer);
        bool rightHitGround = Physics.Raycast(rightRayOrigin, Vector3.down, out rightHit, groundCheckDistance, groundLayer);
        
        if (debugMode)
        {
            Debug.DrawRay(leftRayOrigin, Vector3.down * groundCheckDistance, leftHitGround ? Color.green : Color.red);
            Debug.DrawRay(rightRayOrigin, Vector3.down * groundCheckDistance, rightHitGround ? Color.green : Color.red);
        }
        
        if (leftHitGround && rightHitGround)
        {
            // Calculate angle based on the difference in hit points
            Vector3 groundDirection = rightHit.point - leftHit.point;
            float angle = Mathf.Atan2(groundDirection.y, groundDirection.x) * Mathf.Rad2Deg;
            
            // Clamp the angle to prevent excessive rotation
            return Mathf.Clamp(angle, -leanAngle, leanAngle);
        }
        else if (leftHitGround || rightHitGround)
        {
            // If only one ray hits, use the normal of that hit
            Vector3 hitNormal = leftHitGround ? leftHit.normal : rightHit.normal;
            float angle = Vector3.SignedAngle(Vector3.up, hitNormal, Vector3.forward);
            
            // Invert angle for proper lean direction
            return Mathf.Clamp(-angle, -leanAngle, leanAngle);
        }
        
        return 0f;
    }
    
    void OnDrawGizmosSelected()
    {
        if (!debugMode) return;
        
        Gizmos.color = Color.yellow;
        
        // Draw ground check sphere
        if (playerCollider != null)
        {
            float checkRadius = playerCollider.radius - 0.02f;
            Vector3 groundCheckPos = transform.position + Vector3.down * (playerCollider.height / 2f - playerCollider.radius + 0.1f);
            Gizmos.DrawWireSphere(groundCheckPos, checkRadius);
        }
        
        // Draw raycast positions
        Gizmos.color = Color.cyan;
        Vector3 leftRayOrigin = transform.position + Vector3.left * raycastOffset;
        Vector3 rightRayOrigin = transform.position + Vector3.right * raycastOffset;
        
        Gizmos.DrawLine(leftRayOrigin, leftRayOrigin + Vector3.down * groundCheckDistance);
        Gizmos.DrawLine(rightRayOrigin, rightRayOrigin + Vector3.down * groundCheckDistance);
    }
}