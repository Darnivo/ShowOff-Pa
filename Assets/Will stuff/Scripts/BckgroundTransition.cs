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
        public float yOffset = 0f;
        public float xOffset = 0f;

        [Header("Advanced")]
        public int poolSize = 3;
        public int sortingOrderOverride = -1;
    }

    [System.Serializable]
    public class BackgroundTheme
    {
        [Header("Theme Info")]
        public string themeName;
        public LayerData[] layers;
        public Color ambientTint = Color.white;
        public float globalSpeedMultiplier = 1f;

        public bool IsLayerEnabled(int layerIndex) => 
            layerIndex < layers.Length && layers[layerIndex].enabled && layers[layerIndex].prefab != null;
        
        public LayerData GetLayer(int layerIndex) => 
            layerIndex < layers.Length ? layers[layerIndex] : null;
        
        public int GetLayerCount() => layers?.Length ?? 0;
    }

    [Header("Settings")]
    public int maxLayersSupported = 8;
    public float defaultLayerWidth = 20f;
    public BackgroundTheme[] themes;
    
    [Header("Transition")]
    public float transitionDuration = 5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool autoTransition = true;
    public float timeBetweenThemes = 10f;
    
    [Header("References")]
    public InfiniteBackgroundScroll scroller;
    public bool showDebugInfo = false;

    private int currentThemeIndex = 0;
    private bool isTransitioning = false;
    private Coroutine autoTransitionCoroutine;
    private List<GameObject> transitioningObjects = new List<GameObject>();

    void Start()
    {
        if (scroller == null) scroller = GetComponent<InfiniteBackgroundScroll>();
        
        if (themes.Length > 0)
        {
            SetupInitialTheme();
            if (autoTransition && themes.Length > 1)
                autoTransitionCoroutine = StartCoroutine(AutoTransitionLoop());
        }
        else
        {
            Debug.LogWarning("No background themes configured!");
        }
    }

    void SetupInitialTheme()
    {
        var theme = themes[currentThemeIndex];
        if (showDebugInfo) Debug.Log($"Setting up initial theme: {theme.themeName}");

        EnsureScrollerLayerCount(theme);
        ConfigureScrollerLayers(theme);
        SpawnInitialObjects();
    }

    void ConfigureScrollerLayers(BackgroundTheme theme)
    {
        for (int i = 0; i < theme.GetLayerCount() && i < scroller.layers.Length; i++)
        {
            var layerData = theme.GetLayer(i);
            var scrollerLayer = scroller.layers[i];

            if (theme.IsLayerEnabled(i))
            {
                scrollerLayer.prefab = layerData.prefab;
                scrollerLayer.scrollSpeed = layerData.scrollSpeed * theme.globalSpeedMultiplier;
                scrollerLayer.spawnOffset = layerData.yOffset;
                scrollerLayer.layerWidth = layerData.layerWidth;
                scrollerLayer.poolSize = layerData.poolSize;
                scrollerLayer.layerName = $"{theme.themeName}_Layer{i}";

                InitializeLayerCollections(scrollerLayer);
            }
            else
            {
                scrollerLayer.prefab = null;
            }
        }
    }

    void InitializeLayerCollections(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        if (layer.pool == null) layer.pool = new Queue<GameObject>();
        if (layer.activeObjects == null) layer.activeObjects = new List<GameObject>();
    }

    void SpawnInitialObjects()
    {
        foreach (var layer in scroller.layers)
        {
            if (layer.prefab == null) continue;

            CreateInitialPool(layer);
            SpawnObjectsToFillScreen(layer);
        }
        
        if (showDebugInfo) Debug.Log("Initial background spawn complete");
    }

    void CreateInitialPool(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        for (int i = 0; i < layer.poolSize; i++)
        {
            var obj = Instantiate(layer.prefab, transform);
            obj.SetActive(false);
            layer.pool.Enqueue(obj);
        }
    }

    void SpawnObjectsToFillScreen(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        float cameraX = scroller.cameraTransform.position.x;
        float spawnX = cameraX - scroller.spawnDistance;
        layer.rightmostPosition = spawnX - layer.layerWidth;

        while (spawnX < cameraX + scroller.spawnDistance + layer.layerWidth && layer.pool.Count > 0)
        {
            var obj = layer.pool.Dequeue();
            obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
            obj.SetActive(true);
            layer.activeObjects.Add(obj);

            SetObjectAlpha(obj, 1f);
            layer.rightmostPosition = spawnX;
            spawnX += layer.layerWidth;
        }
    }

    void EnsureScrollerLayerCount(BackgroundTheme theme)
    {
        int requiredLayers = Mathf.Min(theme.GetLayerCount(), maxLayersSupported);
        if (scroller.layers.Length >= requiredLayers) return;

        var newLayers = new InfiniteBackgroundScroll.ScrollingLayer[requiredLayers];
        
        // Copy existing layers
        for (int i = 0; i < scroller.layers.Length; i++)
            newLayers[i] = scroller.layers[i];

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

    float GetDefaultSpeedForLayer(int layerIndex)
    {
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

        if (showDebugInfo) Debug.Log($"Transitioning from {currentTheme.themeName} to {targetTheme.themeName}");

        transitioningObjects.Clear();
        EnsureScrollerLayerCount(targetTheme);

        var tempLayers = CreateAndSpawnTempLayers(targetTheme);
        
        // Wait for proper coverage
        for (int i = 0; i < 3; i++)
        {
            yield return null;
            UpdateTempLayers(tempLayers, 0f);
        }

        // Fade transition
        yield return StartCoroutine(FadeTransition(tempLayers));

        // Complete transition
        CleanupOldTheme();
        SwapToNewTheme(tempLayers, targetThemeIndex);
        isTransitioning = false;

        if (showDebugInfo) Debug.Log($"Transition complete: {themes[currentThemeIndex].themeName}");
    }

    InfiniteBackgroundScroll.ScrollingLayer[] CreateAndSpawnTempLayers(BackgroundTheme targetTheme)
    {
        int maxLayers = Mathf.Max(themes[currentThemeIndex].GetLayerCount(), targetTheme.GetLayerCount());
        maxLayers = Mathf.Min(maxLayers, maxLayersSupported);
        
        var tempLayers = new InfiniteBackgroundScroll.ScrollingLayer[maxLayers];

        for (int i = 0; i < maxLayers; i++)
        {
            if (!targetTheme.IsLayerEnabled(i)) continue;

            var layerData = targetTheme.GetLayer(i);
            tempLayers[i] = CreateTempLayer(layerData, targetTheme, i);
            InitializeTempLayer(tempLayers[i], layerData, i);
            SpawnTempLayerObjects(tempLayers[i]);
        }

        return tempLayers;
    }

    InfiniteBackgroundScroll.ScrollingLayer CreateTempLayer(LayerData layerData, BackgroundTheme theme, int index)
    {
        return new InfiniteBackgroundScroll.ScrollingLayer
        {
            layerName = $"Transition_{theme.themeName}_Layer{index}",
            prefab = layerData.prefab,
            scrollSpeed = layerData.scrollSpeed * theme.globalSpeedMultiplier,
            layerWidth = layerData.layerWidth,
            poolSize = layerData.poolSize,
            spawnOffset = layerData.yOffset,
            pool = new Queue<GameObject>(),
            activeObjects = new List<GameObject>()
        };
    }

    void InitializeTempLayer(InfiniteBackgroundScroll.ScrollingLayer layer, LayerData layerData, int index)
    {
        // Don't pre-create any objects - create them only as needed during spawning
        layer.rightmostPosition = scroller.cameraTransform.position.x - layer.layerWidth * 2;
    }

    void SpawnTempLayerObjects(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        float cameraX = scroller.cameraTransform.position.x;
        float spawnX = cameraX - scroller.spawnDistance - layer.layerWidth;
        float endX = cameraX + scroller.spawnDistance + (layer.layerWidth * 2);
        
        while (spawnX < endX)
        {
            if (layer.pool.Count == 0)
            {
                CreateSinglePoolObject(layer);
            }

            if (layer.pool.Count > 0)
            {
                var obj = layer.pool.Dequeue();
                obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
                obj.SetActive(true);
                layer.activeObjects.Add(obj);
                layer.rightmostPosition = Mathf.Max(layer.rightmostPosition, spawnX);
            }

            spawnX += layer.layerWidth;
        }
    }

    void CreateSinglePoolObject(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        var obj = Instantiate(layer.prefab, transform);
        SetObjectAlpha(obj, 0f);
        obj.SetActive(false);
        layer.pool.Enqueue(obj);
        transitioningObjects.Add(obj);
    }

    IEnumerator FadeTransition(InfiniteBackgroundScroll.ScrollingLayer[] tempLayers)
    {
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(progress);

            // Fade out old theme
            UpdateAllLayersAlpha(scroller.layers, 1f - curveValue);
            
            // Fade in new theme
            UpdateAllLayersAlpha(tempLayers, curveValue);
            
            // Continue movement
            UpdateTempLayers(tempLayers, curveValue);

            yield return null;
        }

        // Ensure final values
        UpdateAllLayersAlpha(scroller.layers, 0f);
        UpdateAllLayersAlpha(tempLayers, 1f);
    }

    void UpdateAllLayersAlpha(InfiniteBackgroundScroll.ScrollingLayer[] layers, float alpha)
    {
        foreach (var layer in layers)
        {
            if (layer?.prefab != null)
                UpdateLayerAlpha(layer, alpha);
        }
    }

    void UpdateTempLayers(InfiniteBackgroundScroll.ScrollingLayer[] tempLayers, float currentAlpha = 1f)
    {
        foreach (var layer in tempLayers)
        {
            if (layer?.prefab == null) continue;

            UpdateLayerMovement(layer);
            SpawnNewObjectsIfNeeded(layer, currentAlpha);
        }
    }

    void UpdateLayerMovement(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        float layerSpeed = scroller.globalScrollSpeed * layer.scrollSpeed;
        
        for (int i = layer.activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = layer.activeObjects[i];
            if (obj == null) continue;

            obj.transform.Translate(Vector3.left * layerSpeed * Time.deltaTime);

            if (obj.transform.position.x < scroller.cameraTransform.position.x - scroller.despawnDistance - layer.layerWidth)
            {
                obj.SetActive(false);
                layer.activeObjects.RemoveAt(i);
                layer.pool.Enqueue(obj);
            }
        }
    }

    void SpawnNewObjectsIfNeeded(InfiniteBackgroundScroll.ScrollingLayer layer, float currentAlpha)
    {
        float rightEdge = scroller.cameraTransform.position.x + scroller.spawnDistance;
        if (layer.rightmostPosition < rightEdge)
        {
            if (layer.pool.Count == 0)
            {
                CreateSinglePoolObject(layer);
            }

            if (layer.pool.Count > 0)
            {
                float spawnX = layer.rightmostPosition + layer.layerWidth;
                var obj = layer.pool.Dequeue();
                obj.transform.position = new Vector3(spawnX, layer.spawnOffset, 0);
                obj.SetActive(true);
                layer.activeObjects.Add(obj);
                layer.rightmostPosition = spawnX;
                
                SetObjectAlpha(obj, currentAlpha);
            }
        }
    }

    void UpdateLayerAlpha(InfiniteBackgroundScroll.ScrollingLayer layer, float alpha)
    {
        if (layer?.activeObjects == null) return;

        for (int i = layer.activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = layer.activeObjects[i];
            if (obj != null)
                SetObjectAlpha(obj, alpha);
            else
                layer.activeObjects.RemoveAt(i);
        }
    }

    void SetObjectAlpha(GameObject obj, float alpha)
    {
        var renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                var color = renderer.color;
                color.a = Mathf.Clamp01(alpha);
                renderer.color = color;
            }
        }
    }

    void SetObjectSortingOrder(GameObject obj, int sortingOrder)
    {
        var renderers = obj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
            renderer.sortingOrder = sortingOrder;
    }

    int GetSortingOrder(LayerData layerData, int defaultOrder) =>
        layerData.sortingOrderOverride >= 0 ? layerData.sortingOrderOverride : defaultOrder;

    void CleanupOldTheme()
    {
        foreach (var layer in scroller.layers)
        {
            DestroyLayerObjects(layer.activeObjects);
            DestroyLayerObjects(layer.pool);
        }
    }

    void DestroyLayerObjects(IEnumerable objects)
    {
        if (objects == null) return;
        
        foreach (GameObject obj in objects)
        {
            if (obj != null) DestroyImmediate(obj);
        }
        
        if (objects is List<GameObject> list) list.Clear();
        else if (objects is Queue<GameObject> queue) queue.Clear();
    }

    void SwapToNewTheme(InfiniteBackgroundScroll.ScrollingLayer[] tempLayers, int newThemeIndex)
    {
        currentThemeIndex = newThemeIndex;
        var newTheme = themes[newThemeIndex];

        EnsureScrollerLayerCount(newTheme);

        for (int i = 0; i < tempLayers.Length; i++)
        {
            if (tempLayers[i] != null)
            {
                scroller.layers[i] = tempLayers[i];
                FinalizeLayerObjects(scroller.layers[i], newTheme.GetLayer(i), i);
            }
            else if (i < scroller.layers.Length)
            {
                ClearLayer(scroller.layers[i]);
            }
        }

        transitioningObjects.Clear();
        if (showDebugInfo) Debug.Log($"Theme swap complete. Active layers: {GetActiveLayerCount()}");
    }

    void FinalizeLayerObjects(InfiniteBackgroundScroll.ScrollingLayer layer, LayerData layerData, int index)
    {
        int sortingOrder = GetSortingOrder(layerData, index);
        
        // Clean up null references and set final properties
        for (int i = layer.activeObjects.Count - 1; i >= 0; i--)
        {
            var obj = layer.activeObjects[i];
            if (obj != null)
            {
                SetObjectSortingOrder(obj, sortingOrder);
                SetObjectAlpha(obj, 1f);
            }
            else
            {
                layer.activeObjects.RemoveAt(i);
            }
        }

        // Clean up pool
        var tempPool = new Queue<GameObject>();
        while (layer.pool.Count > 0)
        {
            var obj = layer.pool.Dequeue();
            if (obj != null) tempPool.Enqueue(obj);
        }
        layer.pool = tempPool;
    }

    void ClearLayer(InfiniteBackgroundScroll.ScrollingLayer layer)
    {
        layer.prefab = null;
        layer.activeObjects?.Clear();
        layer.pool?.Clear();
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