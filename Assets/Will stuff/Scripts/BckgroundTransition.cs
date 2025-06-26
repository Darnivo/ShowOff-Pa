using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BckgroundTransition : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundGroup
    {
        public string name;
        public GameObject backgroundParent; // The main background GameObject
        public List<SpriteRenderer> layers = new List<SpriteRenderer>();
        [HideInInspector] public List<Color> originalColors = new List<Color>();
    }
    
    [Header("Background Groups")]
    public BackgroundGroup[] backgrounds;
    
    [Header("Transition Settings")]
    public float transitionDuration = 3f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public bool autoTransition = true;
    public float timeBetweenTransitions = 5f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    private bool isTransitioning = false;
    private int currentBackgroundIndex = 0;
    private Coroutine autoTransitionCoroutine;
    
    void Start()
    {
        InitializeBackgrounds();
        
        // Show first background, hide others
        for (int i = 0; i < backgrounds.Length; i++)
        {
            SetBackgroundOpacity(i, i == 0 ? 1f : 0f);
        }
        
        if (autoTransition && backgrounds.Length > 1)
        {
            autoTransitionCoroutine = StartCoroutine(AutoTransitionLoop());
        }
    }
    
    void InitializeBackgrounds()
    {
        foreach (var bg in backgrounds)
        {
            if (bg.backgroundParent == null) continue;
            
            // Clear existing lists
            bg.layers.Clear();
            bg.originalColors.Clear();
            
            // Find all SpriteRenderers in children (the layer GameObjects)
            SpriteRenderer[] renderers = bg.backgroundParent.GetComponentsInChildren<SpriteRenderer>();
            
            foreach (var renderer in renderers)
            {
                bg.layers.Add(renderer);
                bg.originalColors.Add(renderer.color);
            }
            
            if (showDebugInfo)
            {
                Debug.Log($"Background '{bg.name}' initialized with {bg.layers.Count} layers");
            }
        }
    }
    
    IEnumerator AutoTransitionLoop()
    {
        while (autoTransition && backgrounds.Length > 1)
        {
            yield return new WaitForSeconds(timeBetweenTransitions);
            
            if (!isTransitioning)
            {
                int nextIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
                StartTransition(nextIndex);
            }
        }
    }
    
    public void StartTransition(int targetBackgroundIndex)
    {
        if (isTransitioning || targetBackgroundIndex == currentBackgroundIndex || 
            targetBackgroundIndex < 0 || targetBackgroundIndex >= backgrounds.Length)
            return;
            
        StartCoroutine(TransitionCoroutine(targetBackgroundIndex));
    }
    
    public void StartNextTransition()
    {
        int nextIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
        StartTransition(nextIndex);
    }
    
    IEnumerator TransitionCoroutine(int targetIndex)
    {
        isTransitioning = true;
        int fromIndex = currentBackgroundIndex;
        float elapsed = 0f;
        
        if (showDebugInfo)
        {
            Debug.Log($"Transitioning from {backgrounds[fromIndex].name} to {backgrounds[targetIndex].name}");
        }
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / transitionDuration;
            float curveValue = transitionCurve.Evaluate(normalizedTime);
            
            // Fade out current background
            SetBackgroundOpacity(fromIndex, 1f - curveValue);
            
            // Fade in target background
            SetBackgroundOpacity(targetIndex, curveValue);
            
            yield return null;
        }
        
        // Ensure final values
        SetBackgroundOpacity(fromIndex, 0f);
        SetBackgroundOpacity(targetIndex, 1f);
        
        currentBackgroundIndex = targetIndex;
        isTransitioning = false;
        
        if (showDebugInfo)
        {
            Debug.Log($"Transition complete. Current background: {backgrounds[currentBackgroundIndex].name}");
        }
    }
    
    void SetBackgroundOpacity(int backgroundIndex, float opacity)
    {
        if (backgroundIndex < 0 || backgroundIndex >= backgrounds.Length) return;
        
        var background = backgrounds[backgroundIndex];
        
        for (int i = 0; i < background.layers.Count; i++)
        {
            if (background.layers[i] != null && i < background.originalColors.Count)
            {
                Color newColor = background.originalColors[i];
                newColor.a = background.originalColors[i].a * opacity;
                background.layers[i].color = newColor;
            }
        }
    }
    
    // Public control methods
    public void SetAutoTransition(bool enabled)
    {
        autoTransition = enabled;
        
        if (autoTransitionCoroutine != null)
        {
            StopCoroutine(autoTransitionCoroutine);
            autoTransitionCoroutine = null;
        }
        
        if (enabled && backgrounds.Length > 1)
        {
            autoTransitionCoroutine = StartCoroutine(AutoTransitionLoop());
        }
    }
    
    public void ForceShowBackground(int backgroundIndex)
    {
        if (backgroundIndex < 0 || backgroundIndex >= backgrounds.Length) return;
        
        // Stop any ongoing transition
        if (isTransitioning)
        {
            StopAllCoroutines();
            isTransitioning = false;
        }
        
        // Hide all backgrounds
        for (int i = 0; i < backgrounds.Length; i++)
        {
            SetBackgroundOpacity(i, 0f);
        }
        
        // Show target background
        SetBackgroundOpacity(backgroundIndex, 1f);
        currentBackgroundIndex = backgroundIndex;
        
        // Restart auto transition if enabled
        if (autoTransition && backgrounds.Length > 1)
        {
            autoTransitionCoroutine = StartCoroutine(AutoTransitionLoop());
        }
    }
    
    public void SetTransitionDuration(float duration)
    {
        transitionDuration = Mathf.Max(0.1f, duration);
    }
    
    public string GetCurrentBackgroundName()
    {
        if (currentBackgroundIndex >= 0 && currentBackgroundIndex < backgrounds.Length)
        {
            return backgrounds[currentBackgroundIndex].name;
        }
        return "None";
    }
    
    // Editor helper methods
    [ContextMenu("Refresh Background Layers")]
    void RefreshBackgroundLayers()
    {
        InitializeBackgrounds();
    }
    
    [ContextMenu("Test Next Transition")]
    void TestNextTransition()
    {
        if (Application.isPlaying)
        {
            StartNextTransition();
        }
    }
}