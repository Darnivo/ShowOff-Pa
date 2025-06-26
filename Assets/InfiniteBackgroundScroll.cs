using UnityEngine;
using System.Collections.Generic;

public class InfiniteBackgroundScroll : MonoBehaviour
{
    [System.Serializable]
    public class ScrollingLayer
    {
        [Header("Layer Settings")]
        public string layerName;
        public GameObject prefab; // Your background layer prefab
        public float scrollSpeed = 1f; // Speed multiplier for parallax effect
        public float layerWidth = 20f; // Width of each background piece
        public int poolSize = 3; // How many pieces to keep in pool
        
        [Header("Spawning")]
        public bool spawnMultiplePieces = true; // For seamless backgrounds
        public float spawnOffset = 0f; // Y offset for this layer
        
        [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
        [HideInInspector] public List<GameObject> activeObjects = new List<GameObject>();
        [HideInInspector] public float rightmostPosition;
    }
    
    [Header("Global Settings")]
    public float globalScrollSpeed = 2f;
    public Transform cameraTransform;
    public float despawnDistance = 30f; // How far left of camera to despawn
    public float spawnDistance = 30f; // How far right of camera to spawn
    
    [Header("Layers (Back to Front)")]
    public ScrollingLayer[] layers;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    public bool showGizmos = true;
    
    private float cameraX;
    
    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
            
        InitializeLayers();
    }
    
    void Update()
    {
        cameraX = cameraTransform.position.x;
        
        foreach (var layer in layers)
        {
            UpdateLayer(layer);
        }
    }
    
    void InitializeLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            var layer = layers[i];
            
            // Skip layers without prefabs
            if (layer.prefab == null)
            {
                if (showDebugInfo)
                    Debug.Log($"Skipping layer {i} - no prefab assigned");
                continue;
            }
            
            // Initialize collections if null
            if (layer.pool == null) layer.pool = new Queue<GameObject>();
            if (layer.activeObjects == null) layer.activeObjects = new List<GameObject>();
            
            // Create pool
            for (int j = 0; j < layer.poolSize; j++)
            {
                GameObject obj = CreateLayerObject(layer, i);
                obj.SetActive(false);
                layer.pool.Enqueue(obj);
            }
            
            // Spawn initial objects
            layer.rightmostPosition = cameraX - layer.layerWidth;
            SpawnInitialObjects(layer, i);
        }
    }
    
    GameObject CreateLayerObject(ScrollingLayer layer, int layerIndex)
    {
        GameObject obj = Instantiate(layer.prefab, transform);
        
        // Set sorting order for proper layering (lower index = further back)
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.sortingOrder = layerIndex;
        }
        
        return obj;
    }
    
    void SpawnInitialObjects(ScrollingLayer layer, int layerIndex)
    {
        // Spawn enough objects to fill the screen plus buffer
        float spawnX = cameraX - spawnDistance;
        
        while (spawnX < cameraX + spawnDistance + layer.layerWidth)
        {
            SpawnLayerObject(layer, spawnX, layerIndex);
            spawnX += layer.layerWidth;
        }
    }
    
    void UpdateLayer(ScrollingLayer layer)
    {
        // Skip disabled layers
        if (layer.prefab == null) return;
        
        float layerSpeed = globalScrollSpeed * layer.scrollSpeed;
        
        // Move all active objects
        for (int i = layer.activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = layer.activeObjects[i];
            if (obj != null)
            {
                obj.transform.Translate(Vector3.left * layerSpeed * Time.deltaTime);
                
                // Check if object should be despawned
                if (obj.transform.position.x < cameraX - despawnDistance)
                {
                    DespawnLayerObject(layer, obj);
                }
            }
        }
        
        // Spawn new objects if needed
        while (layer.rightmostPosition < cameraX + spawnDistance)
        {
            SpawnLayerObject(layer, layer.rightmostPosition + layer.layerWidth, System.Array.IndexOf(layers, layer));
        }
    }
    
    void SpawnLayerObject(ScrollingLayer layer, float xPosition, int layerIndex)
    {
        GameObject obj = GetPooledObject(layer, layerIndex);
        
        Vector3 spawnPos = new Vector3(xPosition, layer.spawnOffset, 0);
        obj.transform.position = spawnPos;
        obj.SetActive(true);
        
        layer.activeObjects.Add(obj);
        layer.rightmostPosition = Mathf.Max(layer.rightmostPosition, xPosition);
        
        if (showDebugInfo)
        {
            Debug.Log($"Spawned {layer.layerName} at x: {xPosition}");
        }
    }
    
    void DespawnLayerObject(ScrollingLayer layer, GameObject obj)
    {
        obj.SetActive(false);
        layer.activeObjects.Remove(obj);
        layer.pool.Enqueue(obj);
        
        if (showDebugInfo)
        {
            Debug.Log($"Despawned {layer.layerName}");
        }
    }
    
    GameObject GetPooledObject(ScrollingLayer layer, int layerIndex)
    {
        if (layer.pool.Count > 0)
        {
            return layer.pool.Dequeue();
        }
        
        // Pool is empty, create new object
        if (showDebugInfo)
        {
            Debug.LogWarning($"Pool empty for {layer.layerName}, creating new object");
        }
        
        return CreateLayerObject(layer, layerIndex);
    }
    
    // Public control methods
    public void SetGlobalSpeed(float speed)
    {
        globalScrollSpeed = speed;
    }
    
    public void SetLayerSpeed(int layerIndex, float speed)
    {
        if (layerIndex >= 0 && layerIndex < layers.Length)
        {
            layers[layerIndex].scrollSpeed = speed;
        }
    }
    
    public void PauseScrolling()
    {
        globalScrollSpeed = 0f;
    }
    
    public void ResumeScrolling(float speed = 2f)
    {
        globalScrollSpeed = speed;
    }
    
    // Gizmos for debugging
    void OnDrawGizmos()
    {
        if (!showGizmos || cameraTransform == null) return;
        
        Vector3 camPos = Application.isPlaying ? 
            new Vector3(cameraX, cameraTransform.position.y, cameraTransform.position.z) : 
            cameraTransform.position;
        
        // Draw spawn and despawn boundaries
        Gizmos.color = Color.green;
        Vector3 spawnLine = camPos + Vector3.right * spawnDistance;
        Gizmos.DrawLine(spawnLine + Vector3.up * 10, spawnLine + Vector3.down * 10);
        
        Gizmos.color = Color.red;
        Vector3 despawnLine = camPos + Vector3.left * despawnDistance;
        Gizmos.DrawLine(despawnLine + Vector3.up * 10, despawnLine + Vector3.down * 10);
        
        // Draw camera view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(camPos, new Vector3(20, 10, 0)); // Adjust based on your camera size
    }
}