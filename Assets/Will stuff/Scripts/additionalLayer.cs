using UnityEngine;
using System.Collections.Generic;

public class SimplePersistentLayer : MonoBehaviour
{
    [Header("Layer Setup")]
    public GameObject layerPrefab;
    public float scrollSpeed = 1f;
    public float layerWidth = 20f;
    
    [Header("Position")]
    public float yOffset = 0f;
    
    [Header("Spawn Settings")]
    public float spawnDistance = 30f;
    public float despawnDistance = 30f;
    public int poolSize = 3;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    [Header("References")]
    public Transform cameraTransform;
    
    // Runtime variables
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();
    private float rightmostPosition;
    
    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
            
        if (layerPrefab == null)
        {
            Debug.LogWarning("No prefab assigned to SimplePersistentLayer!");
            return;
        }
        
        InitializeLayer();
    }
    
    void Update()
    {
        if (layerPrefab == null || cameraTransform == null) return;
        
        UpdateLayer();
    }
    
    void InitializeLayer()
    {
        // Create pool of objects
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(layerPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        
        // Set initial spawn position
        float cameraX = cameraTransform.position.x;
        rightmostPosition = cameraX - layerWidth;
        
        // Spawn initial objects to fill the screen
        SpawnInitialObjects();
    }
    
    void SpawnInitialObjects()
    {
        float cameraX = cameraTransform.position.x;
        float spawnX = cameraX - spawnDistance;
        
        while (spawnX < cameraX + spawnDistance + layerWidth)
        {
            SpawnObject(spawnX);
            spawnX += layerWidth;
        }
    }
    
    void UpdateLayer()
    {
        float cameraX = cameraTransform.position.x;
        
        // Move all active objects
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = activeObjects[i];
            if (obj != null)
            {
                // Move the object
                obj.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
                
                // Check if object should be despawned
                if (obj.transform.position.x < cameraX - despawnDistance)
                {
                    DespawnObject(obj);
                }
            }
        }
        
        // Update rightmost position based on actual objects
        UpdateRightmostPosition();
        
        // Continuously spawn new objects ahead of the camera
        float targetSpawnPoint = cameraX + spawnDistance;
        
        if (showDebugInfo && Time.frameCount % 60 == 0) // Log every second
        {
            Debug.Log($"Camera X: {cameraX}, Rightmost: {rightmostPosition}, Target spawn: {targetSpawnPoint}, Should spawn: {rightmostPosition < targetSpawnPoint}");
        }
        
        // Keep spawning until we have enough coverage
        while (rightmostPosition < targetSpawnPoint)
        {
            float nextSpawnX = rightmostPosition + layerWidth;
            SpawnObject(nextSpawnX);
            rightmostPosition = nextSpawnX;
        }
    }
    
    void UpdateRightmostPosition()
    {
        if (activeObjects.Count == 0)
        {
            // No objects, reset to camera position
            rightmostPosition = cameraTransform.position.x - layerWidth;
            return;
        }
        
        // Find the actual rightmost object
        float actualRightmost = float.MinValue;
        foreach (GameObject obj in activeObjects)
        {
            if (obj != null && obj.transform.position.x > actualRightmost)
            {
                actualRightmost = obj.transform.position.x;
            }
        }
        
        if (actualRightmost != float.MinValue)
        {
            rightmostPosition = actualRightmost;
        }
    }
    
    void SpawnObject(float xPosition)
    {
        GameObject obj = GetPooledObject();
        if (obj == null) return;
        
        // Position the object
        obj.transform.position = new Vector3(xPosition, yOffset, 0);
        obj.SetActive(true);
        
        // Add to active list
        activeObjects.Add(obj);
        
        // Debug info
        if (showDebugInfo)
        {
            Debug.Log($"Spawned object at X: {xPosition}, Camera X: {cameraTransform.position.x}, Active objects: {activeObjects.Count}");
        }
    }
    
    void DespawnObject(GameObject obj)
    {
        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
        
        if (showDebugInfo)
        {
            Debug.Log($"Despawned object. Active objects: {activeObjects.Count}, Pool size: {pool.Count}");
        }
    }
    
    GameObject GetPooledObject()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        
        // Pool is empty, create a new object
        return Instantiate(layerPrefab, transform);
    }
    
    // Update Y position of all objects when slider changes
    void OnValidate()
    {
        if (Application.isPlaying && activeObjects != null)
        {
            UpdateAllObjectPositions();
        }
    }
    
    void UpdateAllObjectPositions()
    {
        foreach (GameObject obj in activeObjects)
        {
            if (obj != null)
            {
                Vector3 pos = obj.transform.position;
                pos.y = yOffset;
                obj.transform.position = pos;
            }
        }
    }
    
    [ContextMenu("Force Spawn Object")]
    void ForceSpawnObject()
    {
        if (Application.isPlaying && cameraTransform != null)
        {
            float spawnX = cameraTransform.position.x + spawnDistance;
            SpawnObject(spawnX);
            Debug.Log($"Force spawned object at X: {spawnX}");
        }
    }
    
    [ContextMenu("Reset Layer")]
    void ResetLayer()
    {
        if (Application.isPlaying)
        {
            // Clear all objects
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null)
                    DestroyImmediate(obj);
            }
            activeObjects.Clear();
            
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                    DestroyImmediate(obj);
            }
            
            // Reinitialize
            InitializeLayer();
            Debug.Log("Layer reset and reinitialized");
        }
    }
    [ContextMenu("Log Current Status")]
    void LogCurrentStatus()
    {
        if (cameraTransform != null)
        {
            float cameraX = cameraTransform.position.x;
            Debug.Log($"=== SimplePersistentLayer Status ===");
            Debug.Log($"Camera X: {cameraX}");
            Debug.Log($"Rightmost Position: {rightmostPosition}");
            Debug.Log($"Spawn Distance: {spawnDistance}");
            Debug.Log($"Next spawn should be at: {cameraX + spawnDistance}");
            Debug.Log($"Active Objects: {activeObjects.Count}");
            Debug.Log($"Pool Objects: {pool.Count}");
            Debug.Log($"Layer Width: {layerWidth}");
            
            if (activeObjects.Count > 0)
            {
                Debug.Log("Active object positions:");
                foreach (var obj in activeObjects)
                {
                    if (obj != null)
                        Debug.Log($"  - X: {obj.transform.position.x}");
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        if (cameraTransform == null) return;
        
        Vector3 cameraPos = cameraTransform.position;
        
        // Draw spawn boundary (green)
        Gizmos.color = Color.green;
        Vector3 spawnLine = cameraPos + Vector3.right * spawnDistance;
        spawnLine.y = yOffset;
        Gizmos.DrawLine(spawnLine + Vector3.up * 5, spawnLine + Vector3.down * 5);
        
        // Draw despawn boundary (red)
        Gizmos.color = Color.red;
        Vector3 despawnLine = cameraPos + Vector3.left * despawnDistance;
        despawnLine.y = yOffset;
        Gizmos.DrawLine(despawnLine + Vector3.up * 5, despawnLine + Vector3.down * 5);
        
        // Draw layer level (blue)
        Gizmos.color = Color.blue;
        Vector3 leftPoint = cameraPos + Vector3.left * (spawnDistance + 10);
        Vector3 rightPoint = cameraPos + Vector3.right * (spawnDistance + 10);
        leftPoint.y = rightPoint.y = yOffset;
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}