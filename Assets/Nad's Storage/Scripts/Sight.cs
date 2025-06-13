using UnityEngine;
using System.Collections.Generic; // Required for using Lists
using System.Linq; // Required for Linq operations like ToList

public class SightBox : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private Camera mainCamera;
    private bool _followingPlayer = false;
    public GameObject birdSprite;

    [Header("Mouse Following Settings")]
    [Tooltip("Maximum speed the sight box can move (in screen space per second, like the spotlight)")]
    [Range(0.1f, 10.0f)]
    public float maxFollowSpeed = 2.0f;
    
    [Tooltip("Smoothing factor for sight box movement (lower = more lag, higher = more responsive)")]
    [Range(0.01f, 1.0f)]
    public float smoothingFactor = 0.1f;
    
    [Tooltip("Use speed limit (true) or smoothing only (false)")]
    public bool useSpeedLimit = true;
    
    [Tooltip("Distance threshold to snap to target (prevents tiny movements)")]
    [Range(0.001f, 0.1f)]
    public float snapThreshold = 0.01f;

    private Collider sightCollider; // The collider defining the "box of sight"
    // private List<DreamObject> potentialDreamObjects;
    
    // Smooth following variables
    private Vector3 currentPosition;
    private Vector3 targetPosition;
    private bool positionInitialized = false;

    void Start()
    {
        mainCamera = Camera.main;
        sightCollider = GetComponent<Collider>(); // Use Collider2D for 2D projects

        if (sightCollider == null)
        {
            Debug.LogError("SightBox requires a Collider component to define its bounds. Please add one.");
            enabled = false; // Disable script if no collider
            return;
        }
        
        // Initialize position
        currentPosition = transform.position;
        targetPosition = currentPosition;
        positionInitialized = true;
        
        // The SightBox's collider doesn't strictly need to be a trigger for this specific logic,
        // but it can be if used for other trigger interactions.

        // Find all GameObjects with the "DreamObject" tag and get their DreamObject component
        // GameObject[] dreamObjectGOs = GameObject.FindGameObjectsWithTag("DreamObject");
        // potentialDreamObjects = new List<DreamObject>();
        // foreach (GameObject go in dreamObjectGOs)
        // {
        //     DreamObject dreamObj = go.GetComponent<DreamObject>();
        //     if (dreamObj != null)
        //     {
        //         potentialDreamObjects.Add(dreamObj);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("GameObject " + go.name + " is tagged as DreamObject but missing DreamObject script.");
        //     }
        // }
        // Debug.Log("Found " + potentialDreamObjects.Count + " potential DreamObjects.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _followingPlayer = !_followingPlayer;
            // temporary
            if (_followingPlayer)
            {
                birdSprite.SetActive(false);
            }
            else
            {
                birdSprite.SetActive(true);
                // When switching back to mouse, snap to avoid jarring movement
                if (mainCamera != null)
                {
                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
                    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                    targetPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
                    currentPosition = targetPosition;
                }
            }
        }
        
        SightFollowSettings();
        
        // Apply the calculated position
        transform.position = currentPosition;

        // CheckForDreamObjects();
    }

    private void SightFollowSettings()
    {
        Vector3 previousTarget = targetPosition;
        
        if (_followingPlayer && player != null)
        {
            // When following player, update target position
            targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        }
        else
        {
            if (mainCamera != null)
            {
                // Calculate mouse world position
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                targetPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            }
            else if (!_followingPlayer) // Only log error if not following player and camera is null
            {
                Debug.LogError("Main Camera not found! SightBox cannot follow mouse.");
                return;
            }
        }
        
        // Initialize if needed
        if (!positionInitialized)
        {
            currentPosition = targetPosition;
            positionInitialized = true;
            return;
        }
        
        // Calculate movement in screen space for consistent speed with spotlight
        Vector3 currentScreenPos = mainCamera.WorldToScreenPoint(currentPosition);
        Vector3 targetScreenPos = mainCamera.WorldToScreenPoint(targetPosition);
        
        // Convert to normalized screen coordinates (0-1 range)
        Vector2 currentNormalized = new Vector2(currentScreenPos.x / Screen.width, currentScreenPos.y / Screen.height);
        Vector2 targetNormalized = new Vector2(targetScreenPos.x / Screen.width, targetScreenPos.y / Screen.height);
        
        // Calculate movement in normalized screen space
        Vector2 direction = targetNormalized - currentNormalized;
        float distance = direction.magnitude;
        
        // Skip tiny movements
        if (distance < snapThreshold * 0.01f) // Adjust threshold for screen space
        {
            currentPosition = targetPosition;
            return;
        }
        
        if (useSpeedLimit && distance > snapThreshold * 0.01f)
        {
            // Apply speed limit in screen space
            float maxDistance = maxFollowSpeed * Time.deltaTime;
            
            if (distance > maxDistance)
            {
                // Limit the movement speed in screen space
                direction = direction.normalized * maxDistance;
                currentNormalized += direction;
                
                // Convert back to world position
                Vector3 newScreenPos = new Vector3(currentNormalized.x * Screen.width, 
                                                  currentNormalized.y * Screen.height, 
                                                  currentScreenPos.z);
                currentPosition = mainCamera.ScreenToWorldPoint(newScreenPos);
                currentPosition.z = transform.position.z; // Maintain Z position
            }
            else
            {
                // If within speed limit, apply smoothing
                currentPosition = Vector3.Lerp(currentPosition, targetPosition, smoothingFactor);
            }
        }
        else
        {
            // Just use smoothing without speed limit
            currentPosition = Vector3.Lerp(currentPosition, targetPosition, smoothingFactor * Time.deltaTime * 60f);
        }
    }

    // Utility method to instantly snap to current target
    public void SnapToTarget()
    {
        if (_followingPlayer && player != null)
        {
            currentPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
            targetPosition = currentPosition;
        }
        else if (mainCamera != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            currentPosition = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            targetPosition = currentPosition;
        }
    }
    
    // Get the current lag distance (useful for effects or debugging)
    public float GetLagDistance()
    {
        return Vector3.Distance(currentPosition, targetPosition);
    }
    
    // Check if the sight box is currently lagging behind its target
    public bool IsLagging()
    {
        return GetLagDistance() > snapThreshold;
    }

    /*
    private void CheckForDreamObjects()
    {
        if (sightCollider == null || potentialDreamObjects == null) return;

        Bounds sightBounds = sightCollider.bounds;

        // Iterate backwards if you plan to remove items from the list
        for (int i = potentialDreamObjects.Count - 1; i >= 0; i--)
        {
            DreamObject dreamObj = potentialDreamObjects[i];
            if (dreamObj == null || dreamObj.IsRevealed()) // Skip if already revealed or somehow null
            {
                if (dreamObj != null && dreamObj.IsRevealed())
                {
                    // Optional: Remove revealed objects from the list to optimize
                    // potentialDreamObjects.RemoveAt(i);
                }
                continue;
            }

            // Check if the DreamObject's position is within the SightBox's bounds
            if (sightBounds.Contains(dreamObj.GetPosition()))
            {
                dreamObj.Reveal();
                // Optional: Remove after revealing to avoid further checks on this object
                // potentialDreamObjects.RemoveAt(i);
            }
        }
    }
    */

    // Optional: If you want to see the bounds of your SightBox in the editor
    // void OnDrawGizmos()
    // {
    //     if (sightCollider != null)
    //     {
    //         Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan, semi-transparent
    //         Gizmos.DrawCube(sightCollider.bounds.center, sightCollider.bounds.size);
    //     }
    // }
}