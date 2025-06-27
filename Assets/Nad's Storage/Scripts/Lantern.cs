using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Animations;

public class Lantern : MonoBehaviour
{
    public MultiSpotlightController spotlightManager;
    public GameObject glowGroup;
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private List<float> alphaValues = new List<float>();
    private float timer = 0f;
    [Header("Spotlight Data")]
    public int elementNumber = 1;
    public float spotlightDuration = 1.5f; 
    [Header("Glow Fade Settings")]
    public float delay = 0.2f; 
    public float fadeDuration = 1f;
    private bool fadeIn = false;
    private float radiusVal;
    private float softnessVal;
    private SpotlightData lanternLight;
    void Awake()
    {
        lanternLight = spotlightManager.spotlights[elementNumber];
        radiusVal = lanternLight.radius;
        softnessVal = lanternLight.softness;
        lanternLight.radius = 0;
        lanternLight.softness = 0; 
    }
    void Start()
    {
        Transform[] group = glowGroup.GetComponentsInChildren<Transform>();
        foreach (var obj in group)
        {
            if (obj.TryGetComponent<SpriteRenderer>(out var sprite))
            {
                spriteRenderers.Add(sprite);
                alphaValues.Add(sprite.color.a);
                Color c = sprite.color;
                c.a = 0;
                sprite.color = c;
            }
        }
    }

    void Update()
    {
        if (fadeIn == true)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                float alpha = Mathf.SmoothStep(0f, alphaValues[i], t);
                Color c = spriteRenderers[i].color;
                c.a = alpha;
                spriteRenderers[i].color = c;
            }
            lanternLight.radius = Mathf.SmoothStep(0f, radiusVal, t);
            lanternLight.softness = Mathf.SmoothStep(0f, softnessVal, t); 
        }
    }

    public void TriggerFade()
    {
        fadeIn = true; 
    }

    public void expand()
    {
        

        // spotlightManager.spotlights.add(lanternLight); 
    }
    
    


}