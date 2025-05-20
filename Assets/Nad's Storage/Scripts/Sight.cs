using UnityEngine;
using System.Collections.Generic; // Required for using Lists
using System.Linq; // Required for Linq operations like ToList

public class SightBox : MonoBehaviour
{
  
    public Transform player;
    private Camera mainCamera;
    private bool _followingPlayer = false;

    private Collider sightCollider; // The collider defining the "box of sight"
    // private List<DreamObject> potentialDreamObjects;

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
        }
        SightFollowSettings();
        
        // CheckForDreamObjects();
    }
    

    private void SightFollowSettings()
    {
        if (_followingPlayer && player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

        }
        else
        {
            if (mainCamera != null)
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            }
            else if (!_followingPlayer) // Only log error if not following player and camera is null
            {
                Debug.LogError("Main Camera not found! SightBox cannot follow mouse.");
            }
        }
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
    void OnDrawGizmos()
    {
        if (sightCollider != null)
        {
            Gizmos.color = new Color(0, 1, 1, 0.3f); // Cyan, semi-transparent
            Gizmos.DrawCube(sightCollider.bounds.center, sightCollider.bounds.size);
        }
    }
}