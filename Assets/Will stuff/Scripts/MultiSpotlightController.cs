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
    
    [Header("World Spotlights")]
    public SpotlightData[] spotlights;
    
    [Header("Settings")]
    public int maxSpots = 5;

    void Update()
    {
        Vector2[] spots = new Vector2[maxSpots];
        float[] radii = new float[maxSpots];
        float[] softness = new float[maxSpots];
        float[] intensities = new float[maxSpots];
        int count = 0;

        // Mouse spotlight
        if (enableMouseSpotlight && count < maxSpots)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector2 mouseUV = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            spots[count] = mouseUV;
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
}