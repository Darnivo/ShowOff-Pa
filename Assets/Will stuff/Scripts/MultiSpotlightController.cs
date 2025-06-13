using UnityEngine;

[System.Serializable]
public class SpotlightData
{
    public Transform worldTarget;
    [Range(0.01f, 1.0f)]
    public float radius = 0.2f;
    [Range(0.001f, 0.5f)]
    public float softness = 0.05f;
    [Range(0.0f, 2.0f)]
    [Tooltip("Light intensity. 0 = no light, 1 = normal, >1 = brighter")]
    public float intensity = 1.0f;
    [Tooltip("If true, this spotlight will follow the mouse")]
    public bool isMouseSpotlight = false;
}

public class MultiSpotlightController : MonoBehaviour
{
    [Header("Material")]
    public Material spotlightMat;
    
    [Header("Mouse Spotlight")]
    public bool enableMouseSpotlight = true;
    [Range(0.01f, 1.0f)]
    public float mouseSpotlightRadius = 0.2f;
    [Range(0.001f, 0.5f)]
    public float mouseSpotlightSoftness = 0.05f;
    [Range(0.0f, 2.0f)]
    [Tooltip("Mouse spotlight intensity. 0 = no light, 1 = normal, >1 = brighter")]
    public float mouseSpotlightIntensity = 1.0f;
    
    [Header("Mouse Spotlight Movement")]
    [Tooltip("Maximum speed the spotlight can move (in UV space per second)")]
    [Range(0.1f, 10.0f)]
    public float maxSpotlightSpeed = 2.0f;
    [Tooltip("Smoothing factor for spotlight movement (lower = more lag, higher = more responsive)")]
    [Range(0.01f, 1.0f)]
    public float smoothingFactor = 0.1f;
    [Tooltip("Use speed limit (true) or smoothing only (false)")]
    public bool useSpeedLimit = true;
    
    [Header("World Spotlights")]
    public SpotlightData[] spotlights;
    
    [Header("Settings")]
    public int maxSpots = 5;
    
    // Internal tracking for smooth mouse spotlight
    private Vector2 currentMouseSpotlightPos;
    private Vector2 targetMousePos;
    private bool mouseSpotlightInitialized = false;

    void Start()
    {
        // Initialize mouse spotlight position
        if (enableMouseSpotlight)
        {
            Vector3 mousePos = Input.mousePosition;
            currentMouseSpotlightPos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            targetMousePos = currentMouseSpotlightPos;
            mouseSpotlightInitialized = true;
        }
    }

    void Update()
    {
        Vector2[] spots = new Vector2[maxSpots];
        float[] radii = new float[maxSpots];
        float[] softness = new float[maxSpots];
        float[] intensities = new float[maxSpots];
        int count = 0;

        // Mouse spotlight with lag
        if (enableMouseSpotlight && count < maxSpots)
        {
            // Get current mouse position in UV space
            Vector3 mousePos = Input.mousePosition;
            targetMousePos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            
            // Initialize if needed
            if (!mouseSpotlightInitialized)
            {
                currentMouseSpotlightPos = targetMousePos;
                mouseSpotlightInitialized = true;
            }
            
            // Calculate movement
            Vector2 direction = targetMousePos - currentMouseSpotlightPos;
            float distance = direction.magnitude;
            
            if (useSpeedLimit && distance > 0.001f)
            {
                // Apply speed limit
                float maxDistance = maxSpotlightSpeed * Time.deltaTime;
                
                if (distance > maxDistance)
                {
                    // Limit the movement speed
                    direction = direction.normalized * maxDistance;
                    currentMouseSpotlightPos += direction;
                }
                else
                {
                    // If within speed limit, apply smoothing
                    currentMouseSpotlightPos = Vector2.Lerp(currentMouseSpotlightPos, targetMousePos, smoothingFactor);
                }
            }
            else
            {
                // Just use smoothing without speed limit
                currentMouseSpotlightPos = Vector2.Lerp(currentMouseSpotlightPos, targetMousePos, smoothingFactor * Time.deltaTime * 60f);
            }
            
            // Clamp to screen bounds
            currentMouseSpotlightPos.x = Mathf.Clamp01(currentMouseSpotlightPos.x);
            currentMouseSpotlightPos.y = Mathf.Clamp01(currentMouseSpotlightPos.y);
            
            spots[count] = currentMouseSpotlightPos;
            radii[count] = mouseSpotlightRadius;
            softness[count] = mouseSpotlightSoftness;
            intensities[count] = mouseSpotlightIntensity;
            count++;
        }

        // World spotlights
        Camera cam = Camera.main;
        foreach (SpotlightData spotlight in spotlights)
        {
            if (count >= maxSpots) break;
            if (spotlight.worldTarget == null) continue;

            Vector3 screenPos = cam.WorldToScreenPoint(spotlight.worldTarget.position);

            // Skip if behind the camera
            if (screenPos.z < 0) continue;

            Vector2 worldUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
            spots[count] = worldUV;
            radii[count] = spotlight.radius;
            softness[count] = spotlight.softness;
            intensities[count] = spotlight.intensity;
            count++;
        }

        // Send data to shader
        spotlightMat.SetInt("_SpotCount", count);
        spotlightMat.SetFloat("_Aspect", (float)Screen.width / Screen.height);
        spotlightMat.SetVectorArray("_SpotPositions", ToVector4Array(spots));
        spotlightMat.SetFloatArray("_SpotRadii", radii);
        spotlightMat.SetFloatArray("_SpotSoftness", softness);
        spotlightMat.SetFloatArray("_SpotIntensities", intensities);
    }

    Vector4[] ToVector4Array(Vector2[] arr)
    {
        Vector4[] v4 = new Vector4[arr.Length];
        for (int i = 0; i < arr.Length; i++)
            v4[i] = new Vector4(arr[i].x, arr[i].y, 0, 0);
        return v4;
    }
    
    // Utility method to animate spotlight intensity at runtime
    public void SetSpotlightIntensity(int spotlightIndex, float intensity)
    {
        if (spotlightIndex >= 0 && spotlightIndex < spotlights.Length)
        {
            spotlights[spotlightIndex].intensity = Mathf.Clamp(intensity, 0f, 2f);
        }
    }
    
    // Utility method to animate mouse spotlight intensity
    public void SetMouseSpotlightIntensity(float intensity)
    {
        mouseSpotlightIntensity = Mathf.Clamp(intensity, 0f, 2f);
    }
    
    // Utility method to instantly snap the spotlight to mouse position
    public void SnapToMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        currentMouseSpotlightPos = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
        targetMousePos = currentMouseSpotlightPos;
    }
    
    // Get the current spotlight position (useful for debugging or other systems)
    public Vector2 GetCurrentSpotlightPosition()
    {
        return currentMouseSpotlightPos;
    }
    
    // Get the distance between spotlight and mouse (useful for effects)
    public float GetSpotlightLagDistance()
    {
        return Vector2.Distance(currentMouseSpotlightPos, targetMousePos);
    }
}