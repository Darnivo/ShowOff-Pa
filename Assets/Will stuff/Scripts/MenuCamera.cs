using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 position1 = new Vector3(-10f, 0f, -10f);
    public Vector3 position2 = new Vector3(10f, 0f, -10f);
    public float movementDuration = 5f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Timing")]
    public float pauseAtPositions = 2f; // Time to pause at each position
    public bool startMovingImmediately = true;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    public bool showGizmos = true;
    
    private bool isMoving = false;
    private bool movingToPosition2 = true; // true = moving to pos2, false = moving to pos1
    private float moveTimer = 0f;
    private float pauseTimer = 0f;
    private Vector3 startPos, targetPos;
    
    void Start()
    {
        // Set initial position
        transform.position = position1;
        
        if (startMovingImmediately)
        {
            StartMovement();
        }
    }
    
    void Update()
    {
        if (isMoving)
        {
            UpdateMovement();
        }
        else if (pauseAtPositions > 0)
        {
            UpdatePause();
        }
        else
        {
            // No pause, start next movement immediately
            StartMovement();
        }
    }
    
    void UpdateMovement()
    {
        moveTimer += Time.deltaTime;
        float progress = moveTimer / movementDuration;
        
        if (progress >= 1f)
        {
            // Movement complete
            progress = 1f;
            transform.position = targetPos;
            isMoving = false;
            moveTimer = 0f;
            pauseTimer = 0f;
            
            if (showDebugInfo)
            {
                Debug.Log($"Reached {(movingToPosition2 ? "Position 2" : "Position 1")}");
            }
            
            return;
        }
        
        // Apply curve to progress
        float curvedProgress = movementCurve.Evaluate(progress);
        
        // Interpolate position
        transform.position = Vector3.Lerp(startPos, targetPos, curvedProgress);
    }
    
    void UpdatePause()
    {
        pauseTimer += Time.deltaTime;
        
        if (pauseTimer >= pauseAtPositions)
        {
            StartMovement();
        }
    }
    
    void StartMovement()
    {
        isMoving = true;
        moveTimer = 0f;
        
        // Set start and target positions
        if (movingToPosition2)
        {
            startPos = position1;
            targetPos = position2;
        }
        else
        {
            startPos = position2;
            targetPos = position1;
        }
        
        // Make sure we start from the correct position
        transform.position = startPos;
        
        // Toggle direction for next movement
        movingToPosition2 = !movingToPosition2;
        
        if (showDebugInfo)
        {
            Debug.Log($"Starting movement to {(movingToPosition2 ? "Position 1" : "Position 2")}");
        }
    }
    
    // Public control methods
    public void StartAutoMovement()
    {
        if (!isMoving)
        {
            StartMovement();
        }
    }
    
    public void StopMovement()
    {
        isMoving = false;
        moveTimer = 0f;
        pauseTimer = 0f;
    }
    
    public void SetPositions(Vector3 pos1, Vector3 pos2)
    {
        position1 = pos1;
        position2 = pos2;
    }
    
    public void SetMovementDuration(float duration)
    {
        movementDuration = Mathf.Max(0.1f, duration);
    }
    
    public void TeleportToPosition1()
    {
        StopMovement();
        transform.position = position1;
        movingToPosition2 = true;
    }
    
    public void TeleportToPosition2()
    {
        StopMovement();
        transform.position = position2;
        movingToPosition2 = false;
    }
    
    // Gizmos for editor visualization
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // Draw positions
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position1, 0.5f);
        Gizmos.DrawIcon(position1, "Camera Icon", true);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position2, 0.5f);
        Gizmos.DrawIcon(position2, "Camera Icon", true);
        
        // Draw path
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(position1, position2);
        
        // Draw current position if playing
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}