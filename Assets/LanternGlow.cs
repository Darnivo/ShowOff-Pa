using UnityEngine;

public class LanternGlow : MonoBehaviour
{
    private SpriteRenderer sr;
    void Start() => sr = GetComponent<SpriteRenderer>();

    void Update() {
        float flicker = Mathf.Lerp(0.6f, 1f, Mathf.PerlinNoise(Time.time * 5f, 0f));
        sr.color = new Color(1f, 0.8f, 0.4f, flicker); // Slight yellow-orange with flicker
    }
}
