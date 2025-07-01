using UnityEngine;
using TMPro;

public class Credits : MonoBehaviour
{
    public float scrollSpeed = 50f;
    public float resetPosition = -1000f; 
    
    private RectTransform rectTransform;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
 
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        
   
        if (rectTransform.anchoredPosition.y > resetPosition)
        {
            rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        }
    }
}