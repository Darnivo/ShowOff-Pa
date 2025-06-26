using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundThemeManager : MonoBehaviour
{
    [System.Serializable]
    public class LayerData
    {
        [Header("Layer Content")]
        public GameObject prefab;
        public bool enabled = true;

        [Header("Movement")]
        public float scrollSpeed = 1f;
        public float layerWidth = 20f;

        [Header("Positioning")]
        public float yOffset = 0f; // Y position offset for this layer
        public float xOffset = 0f; // X position offset (for staggered spawning)

        [Header("Advanced")]
        [Tooltip("Pool size for this layer")]
        public int poolSize = 3;
        [Tooltip("Custom sorting order override (-1 to use layer index)")]
        public int sortingOrderOverride = -1;
    }

    [System.Serializable]
    public class BackgroundTheme
    {
        [Header("Theme Info")]
        public string themeName;

        [Header("Layers (Back to Front)")]
        public LayerData[] layers;

        [Header("Theme Settings")]
        public Color ambientTint = Color.white;
        public float globalSpeedMultiplier = 1f;

        // Helper methods
        public bool IsLayerEnabled(int layerIndex)
        {
            return layerIndex < layers.Length && layers[layerIndex].enabled && layers[layerIndex].prefab != null;
        }

        public LayerData GetLayer(int layerIndex)
        {
            return layerIndex < layers.Length ? layers[layerIndex] : null;
        }

        public int GetLayerCount()
        {
            return layers != null ? layers.Length : 0;
        }
    }

    [Header("Layer Management")]
    public int maxLayersSupported = 8; // Maximum layers any theme can have
    public float defaultLayerWidth = 20f;

    [Header("Background Themes")]
    public BackgroundTheme[] themes;

    [Header("Transition Settings")]
    public float transitionDuration = 5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool autoTransition = true;
    public float timeBetweenThemes = 10f;

    [Header("References")]
    public InfiniteBackgroundScroll scroller;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private int currentThemeIndex = 0;
    private bool isTransitioning = false;
    private Coroutine autoTransitionCoroutine;

    // Store transitioning objects
    private List<GameObject> transitioningObjects = new List<GameObject>();

    void Start()
    {
        if (scroller == null)
            scroller = GetComponent<InfiniteBackgroundScroll>();

        if (themes.Length > 0)
        {
            // Initialize the first theme immediately
            SetupInitialTheme();

            if (autoTransition && themes.Length > 1)
            {
                autoTransitionCoroutine = StartCoroutine(AutoTransitionLoop());
            }
        }
        else
        {
            Debug.LogWarning("No background themes configured!");
        }
    }

    void SetupInitialTheme()
    {
        var theme = themes[currentThemeIndex];

        if (showDebugInfo)
        {
            Debug.Log($"Setting up initial theme: {theme.themeName}");
        }

        // Ensure scroller has enough layers
        EnsureScrollerLayerCount(theme);

        // Update scroller layers with current theme
        int maxLayers = theme.GetLayerCount();
        for (int i = 0; i < maxLayers; i++)
        {
            if (i < scroller.layers.Length)
            {
                var layerData = theme.GetLayer(i);
                if (theme.IsLayerEnabled(i))
                {
                    // Configure active layer
                    scroller.layers[i].prefab = layerData.prefab;
                    scroller.layers[i].scrollSpeed = layerData.scrollSpeed * theme.globalSpeedMultiplier;
                    scroller.layers[i].spawnOffset = layerData.yOffset;
                    scroller.layers[i].layerWidth = layerData.layerWidth;
                    scroller.layers[i].poolSize = layerData.poolSize;
                    scroller.layers[i].layerName = $"{theme.themeName}_Layer{i}";

                    // Initialize layer collections if they don't exist
                    if (scroller.layers[i].pool == null)
                        scroller.layers[i].pool = new Queue<GameObject>();
                    if (scroller.layers[i].activeObjects == null)
                        scroller.layers[i].activeObjects = new List<GameObject>();

                    if (showDebugInfo)
                    {
                        Debug.Log($"Configured layer {i}: {layerData.prefab.name} at Y offset {layerData.yOffset}");
                    }
                }
                else
                {
                    // Disable layer by setting prefab to null
                    scroller.layers[i].prefab = null;
                }
            }
        }

        // Force initial background spawn and visibility
        ForceInitialBackgroundSpawn();
    }

    void ForceInitialBackgroundSpawn()
    {
        // Manually spawn initial background objects to ensure immediate visibility
        for (int i = 0; i < scroller.layers.Length; i++)
        {
            var layer = scroller.layers[i];
            if (layer.prefab != null)
            {
                // Create initial pool
                for (int j = 0; j < layer.poolSize; j++)
                {
                    GameObject obj = Instantiate(layer.prefab, transform);
                    obj.SetActive(false);
                    layer.pool.Enqueue(obj);
                }

                // Spawn objects to fill the screen
                float cameraX = scroller.cameraTransform.position.x;
                float spawnX = cameraX - scroller.spawnDistance;
                layer.rightmostPosition = spawnX - layer.layerWidth;

                while (spawnX < cameraX + scroller.spawnDistance + layer.layerWidth)
                {
                    if (layer.pool.Count > 0)
                    {
                        GameObject obj = layer.pool.Dequeue();
                        obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
                        obj.SetActive(true);
                        layer.activeObjects.Add(obj);

                        // Ensure full visibility immediately
                        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var renderer in renderers)
                        {
                            Color color = renderer.color;
                            color.a = 1f;
                            renderer.color = color;
                        }

                        layer.rightmostPosition = spawnX;
                        spawnX += layer.layerWidth;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        if (showDebugInfo)
        {
            Debug.Log("Initial background spawn complete");
        }
    }

    void EnsureScrollerLayerCount(BackgroundTheme theme)
    {
        int requiredLayers = theme.GetLayerCount();
        requiredLayers = Mathf.Min(requiredLayers, maxLayersSupported);

        if (scroller.layers.Length < requiredLayers)
        {
            // Expand the scroller layers array
            var newLayers = new InfiniteBackgroundScroll.ScrollingLayer[requiredLayers];

            // Copy existing layers
            for (int i = 0; i < scroller.layers.Length; i++)
            {
                newLayers[i] = scroller.layers[i];
            }

            // Create new layers
            for (int i = scroller.layers.Length; i < requiredLayers; i++)
            {
                newLayers[i] = new InfiniteBackgroundScroll.ScrollingLayer
                {
                    layerName = $"Layer_{i}",
                    scrollSpeed = GetDefaultSpeedForLayer(i),
                    layerWidth = defaultLayerWidth,
                    poolSize = 3,
                    spawnMultiplePieces = true,
                    spawnOffset = 0f,
                    pool = new Queue<GameObject>(),
                    activeObjects = new List<GameObject>()
                };
            }

            scroller.layers = newLayers;
        }
    }

    float GetDefaultSpeedForLayer(int layerIndex)
    {
        // Default parallax speeds: further layers are slower
        float[] defaultSpeeds = { 0.1f, 0.25f, 0.4f, 0.6f, 0.8f, 1.0f, 1.2f, 1.5f };
        return layerIndex < defaultSpeeds.Length ? defaultSpeeds[layerIndex] : 1.0f;
    }

    IEnumerator AutoTransitionLoop()
    {
        while (autoTransition && themes.Length > 1)
        {
            yield return new WaitForSeconds(timeBetweenThemes);

            if (!isTransitioning)
            {
                int nextTheme = (currentThemeIndex + 1) % themes.Length;
                StartTransition(nextTheme);
            }
        }
    }

    public void StartTransition(int targetThemeIndex)
    {
        if (isTransitioning || targetThemeIndex == currentThemeIndex ||
            targetThemeIndex < 0 || targetThemeIndex >= themes.Length)
            return;

        StartCoroutine(TransitionCoroutine(targetThemeIndex));
    }

    IEnumerator TransitionCoroutine(int targetThemeIndex)
    {
        isTransitioning = true;
        var targetTheme = themes[targetThemeIndex];
        var currentTheme = themes[currentThemeIndex];

        if (showDebugInfo)
        {
            Debug.Log($"Starting transition from {currentTheme.themeName} to {targetTheme.themeName}");
        }

        // Clear previous transition objects list
        transitioningObjects.Clear();

        // Ensure we have enough layers for the target theme
        EnsureScrollerLayerCount(targetTheme);

        // Create temporary layers for the new theme
        int maxLayers = Mathf.Max(currentTheme.GetLayerCount(), targetTheme.GetLayerCount());
        maxLayers = Mathf.Min(maxLayers, maxLayersSupported);

        var tempScrollers = new InfiniteBackgroundScroll.ScrollingLayer[maxLayers];
        float currentTransitionAlpha = 0f; // Track current alpha for newly spawned objects

        // Initialize and spawn new theme objects BEFORE starting fade
        for (int i = 0; i < maxLayers; i++)
        {
            if (targetTheme.IsLayerEnabled(i))
            {
                var layerData = targetTheme.GetLayer(i);
                tempScrollers[i] = new InfiniteBackgroundScroll.ScrollingLayer
                {
                    layerName = $"Transition_{targetTheme.themeName}_Layer{i}",
                    prefab = layerData.prefab,
                    scrollSpeed = layerData.scrollSpeed * targetTheme.globalSpeedMultiplier,
                    layerWidth = layerData.layerWidth,
                    poolSize = layerData.poolSize,
                    spawnOffset = layerData.yOffset,
                    pool = new Queue<GameObject>(),
                    activeObjects = new List<GameObject>()
                };

                // Initialize the temp layer with same sorting order (will be behind due to spawn order)
                InitializeTempLayer(tempScrollers[i], GetSortingOrder(layerData, i));

                // Spawn objects immediately and ensure they cover the screen
                SpawnTempLayerObjects(tempScrollers[i]);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Pre-spawned {tempScrollers[i].activeObjects.Count} objects for layer {i}");
                }
            }
        }

        // Wait a few frames and continue spawning to ensure full coverage
        for (int frame = 0; frame < 3; frame++)
        {
            yield return null;
            UpdateTempLayers(tempScrollers, 0f); // Keep alpha at 0 but continue spawning
        }

        // Start transition fade
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(progress);
            currentTransitionAlpha = curveValue; // Store for newly spawned objects

            // Update alpha for old theme layers (fade out)
            for (int i = 0; i < scroller.layers.Length; i++)
            {
                if (scroller.layers[i].prefab != null)
                {
                    UpdateLayerAlpha(scroller.layers[i], 1f - curveValue);
                }
            }

            // Update alpha for new theme layers (fade in)
            for (int i = 0; i < tempScrollers.Length; i++)
            {
                if (tempScrollers[i] != null)
                {
                    UpdateLayerAlpha(tempScrollers[i], curveValue);
                }
            }

            // Continue moving both old and new objects with current alpha
            UpdateTempLayers(tempScrollers, currentTransitionAlpha);

            yield return null;
        }

        // Ensure final alpha values are exact
        for (int i = 0; i < scroller.layers.Length; i++)
        {
            if (scroller.layers[i].prefab != null)
            {
                UpdateLayerAlpha(scroller.layers[i], 0f);
            }
        }

        for (int i = 0; i < tempScrollers.Length; i++)
        {
            if (tempScrollers[i] != null)
            {
                UpdateLayerAlpha(tempScrollers[i], 1f);
            }
        }

        // Transition complete - swap themes
        CleanupOldTheme();
        SwapToNewTheme(tempScrollers, targetThemeIndex);

        isTransitioning = false;

        if (showDebugInfo)
        {
            Debug.Log($"Transition complete. Current theme: {themes[currentThemeIndex].themeName}");
        }
    }

    int GetSortingOrder(LayerData layerData, int defaultOrder)
    {
        return layerData.sortingOrderOverride >= 0 ? layerData.sortingOrderOverride : defaultOrder;
    }

    void InitializeTempLayer(InfiniteBackgroundScroll.ScrollingLayer layer, int sortingOrder)
    {
        // Create larger initial pool to ensure we have enough objects
        int initialPoolSize = Mathf.Max(layer.poolSize, 6); // Minimum 6 objects
        
        for (int j = 0; j < initialPoolSize; j++)
        {
            GameObject obj = Instantiate(layer.prefab, transform);

            // Set sorting order and make invisible initially
            SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sortingOrder = sortingOrder;
                // Start with alpha 0 for fade-in
                Color color = renderer.color;
                color.a = 0f;
                renderer.color = color;
            }

            obj.SetActive(false);
            layer.pool.Enqueue(obj);
            transitioningObjects.Add(obj); // Track for cleanup
        }

        layer.rightmostPosition = scroller.cameraTransform.position.x - layer.layerWidth * 2; // Start further back
    }

    void SpawnTempLayerObjects(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        Transform cameraTransform = scroller.cameraTransform;
        float cameraX = cameraTransform.position.x;
        float spawnX = cameraX - scroller.spawnDistance - layer.layerWidth; // Start further back

        // Spawn enough objects to fill the screen plus extra buffer
        float endX = cameraX + scroller.spawnDistance + (layer.layerWidth * 2);
        
        while (spawnX < endX)
        {
            if (layer.pool.Count > 0)
            {
                GameObject obj = layer.pool.Dequeue();
                obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
                obj.SetActive(true);
                layer.activeObjects.Add(obj);

                layer.rightmostPosition = Mathf.Max(layer.rightmostPosition, spawnX);
                
                if (showDebugInfo)
                {
                    Debug.Log($"Spawned {layer.layerName} object at x: {spawnX}");
                }
            }
            else
            {
                // If pool is empty, create more objects
                for (int j = 0; j < layer.poolSize; j++)
                {
                    GameObject newObj = Instantiate(layer.prefab, transform);
                    
                    // Set sorting order and make invisible initially
                    SpriteRenderer[] renderers = newObj.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var renderer in renderers)
                    {
                        // Use the same sorting order as current theme (new objects will appear behind due to creation order)
                        Color color = renderer.color;
                        color.a = 0f;
                        renderer.color = color;
                    }
                    
                    newObj.SetActive(false);
                    layer.pool.Enqueue(newObj);
                    transitioningObjects.Add(newObj);
                }
                
                if (showDebugInfo)
                {
                    Debug.Log($"Created additional pool objects for {layer.layerName}");
                }
            }

            spawnX += layer.layerWidth;
        }
    }

    void UpdateTempLayers(InfiniteBackgroundScroll.ScrollingLayer[] tempLayers, float currentAlpha = 1f)
    {
        for (int i = 0; i < tempLayers.Length; i++)
        {
            if (tempLayers[i] != null)
            {
                var layer = tempLayers[i];
                float layerSpeed = scroller.globalScrollSpeed * layer.scrollSpeed;

                // Move objects
                for (int j = layer.activeObjects.Count - 1; j >= 0; j--)
                {
                    var obj = layer.activeObjects[j];
                    if (obj != null)
                    {
                        obj.transform.Translate(Vector3.left * layerSpeed * Time.deltaTime);

                        // Check if object is out of bounds and recycle it
                        if (obj.transform.position.x < scroller.cameraTransform.position.x - scroller.despawnDistance - layer.layerWidth)
                        {
                            obj.SetActive(false);
                            layer.activeObjects.RemoveAt(j);
                            layer.pool.Enqueue(obj);
                        }
                    }
                }

                // Spawn new objects if needed
                float rightEdge = scroller.cameraTransform.position.x + scroller.spawnDistance;
                if (layer.rightmostPosition < rightEdge)
                {
                    float spawnX = layer.rightmostPosition + layer.layerWidth;
                    if (layer.pool.Count > 0)
                    {
                        GameObject obj = layer.pool.Dequeue();
                        obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
                        obj.SetActive(true);
                        layer.activeObjects.Add(obj);
                        layer.rightmostPosition = spawnX;

                        // Set alpha for newly spawned objects to match current transition state
                        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var renderer in renderers)
                        {
                            Color color = renderer.color;
                            color.a = currentAlpha;
                            renderer.color = color;
                        }
                    }
                }
            }
        }
    }

    void UpdateLayerAlpha(InfiniteBackgroundScroll.ScrollingLayer layer, float alpha)
    {
        if (layer == null || layer.activeObjects == null) return;

        for (int i = layer.activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = layer.activeObjects[i];
            if (obj != null)
            {
                SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    if (renderer != null)
                    {
                        Color color = renderer.color;
                        color.a = Mathf.Clamp01(alpha);
                        renderer.color = color;
                    }
                }
            }
            else
            {
                // Remove null references from the list
                layer.activeObjects.RemoveAt(i);
            }
        }
    }

    void CleanupOldTheme()
    {
        // Destroy all objects from the old theme
        foreach (var layer in scroller.layers)
        {
            if (layer.activeObjects != null)
            {
                foreach (var obj in layer.activeObjects)
                {
                    if (obj != null)
                        DestroyImmediate(obj);
                }
                layer.activeObjects.Clear();
            }

            if (layer.pool != null)
            {
                while (layer.pool.Count > 0)
                {
                    GameObject obj = layer.pool.Dequeue();
                    if (obj != null)
                        DestroyImmediate(obj);
                }
                layer.pool.Clear();
            }
        }

        // DON'T clean up transitioning objects here - they're now part of the new theme
        // We'll clear the list in SwapToNewTheme after the transfer is complete
    }

    void SwapToNewTheme(InfiniteBackgroundScroll.ScrollingLayer[] tempLayers, int newThemeIndex)
    {
        currentThemeIndex = newThemeIndex;
        var newTheme = themes[newThemeIndex];

        // Ensure scroller has the right number of layers
        if (scroller.layers.Length < tempLayers.Length)
        {
            var expandedLayers = new InfiniteBackgroundScroll.ScrollingLayer[tempLayers.Length];
            for (int i = 0; i < scroller.layers.Length; i++)
            {
                expandedLayers[i] = scroller.layers[i];
            }
            for (int i = scroller.layers.Length; i < tempLayers.Length; i++)
            {
                expandedLayers[i] = new InfiniteBackgroundScroll.ScrollingLayer();
            }
            scroller.layers = expandedLayers;
        }

        // Update scroller layers
        for (int i = 0; i < tempLayers.Length; i++)
        {
            if (tempLayers[i] != null)
            {
                scroller.layers[i] = tempLayers[i];
                var layerData = newTheme.GetLayer(i);

                // Reset sorting orders to normal - check for null objects first
                for (int j = scroller.layers[i].activeObjects.Count - 1; j >= 0; j--)
                {
                    var obj = scroller.layers[i].activeObjects[j];
                    if (obj != null)
                    {
                        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();
                        foreach (var renderer in renderers)
                        {
                            if (renderer != null)
                            {
                                renderer.sortingOrder = GetSortingOrder(layerData, i);
                                // Ensure full opacity
                                Color color = renderer.color;
                                color.a = 1f;
                                renderer.color = color;
                            }
                        }
                    }
                    else
                    {
                        // Remove null references
                        scroller.layers[i].activeObjects.RemoveAt(j);
                    }
                }

                // Also clean up pool of any null references
                var tempPool = new Queue<GameObject>();
                while (scroller.layers[i].pool.Count > 0)
                {
                    var poolObj = scroller.layers[i].pool.Dequeue();
                    if (poolObj != null)
                    {
                        tempPool.Enqueue(poolObj);
                    }
                }
                scroller.layers[i].pool = tempPool;
            }
            else if (i < scroller.layers.Length)
            {
                // Clear layers that don't exist in the new theme
                scroller.layers[i].prefab = null;
                scroller.layers[i].activeObjects?.Clear();
                scroller.layers[i].pool?.Clear();
            }
        }

        transitioningObjects.Clear();

        if (showDebugInfo)
        {
            Debug.Log($"Theme swap complete. Active layers: {GetActiveLayerCount()}");
            
            // Debug: Check each layer's status
            for (int i = 0; i < scroller.layers.Length; i++)
            {
                var layer = scroller.layers[i];
                if (layer.prefab != null)
                {
                    Debug.Log($"Layer {i}: {layer.activeObjects.Count} active objects, {layer.pool.Count} pooled objects");
                }
                else
                {
                    Debug.Log($"Layer {i}: No prefab assigned");
                }
            }
        }
    }

    int GetActiveLayerCount()
    {
        int count = 0;
        foreach (var layer in scroller.layers)
        {
            if (layer.prefab != null) count++;
        }
        return count;
    }
}