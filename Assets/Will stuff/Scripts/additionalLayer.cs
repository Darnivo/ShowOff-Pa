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
        
        // Spawn new objects if needed
        while (rightmostPosition < cameraX + spawnDistance)
        {
            SpawnObject(rightmostPosition + layerWidth);
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
        rightmostPosition = Mathf.Max(rightmostPosition, xPosition);
    }
    
    void DespawnObject(GameObject obj)
    {
        obj.SetActive(false);
        activeObjects.Remove(obj);
        pool.Enqueue(obj);
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
    
    // Gizmos to visualize spawn/despawn ranges
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