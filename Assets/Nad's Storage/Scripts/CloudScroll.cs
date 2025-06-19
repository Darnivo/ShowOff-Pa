using UnityEngine;

public class ContinuousClouds : MonoBehaviour
{
    public float speed = 10f;
    private RectTransform cloud;

    void Awake()
    {
        cloud = GetComponent<RectTransform>();
    }
    void Update()
    {
        cloud.anchoredPosition += Vector2.left * speed * Time.unscaledDeltaTime;
        if (cloud.anchoredPosition.x <= -cloud.sizeDelta.x)
        {
            cloud.anchoredPosition = new Vector2(0, cloud.anchoredPosition.y);
        }
    }

}