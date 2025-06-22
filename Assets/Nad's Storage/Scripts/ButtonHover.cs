using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float expandScale = 1.1f; // Scale factor when hovered
    public float scaleSpeed = 0.1f; // Speed of scaling effect
    private bool hovering = false;
    private Vector3 targetScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        
        if (hovering)
        {
            targetScale = originalScale * expandScale;
        }
        else
        {
            targetScale = originalScale;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.unscaledDeltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
    
}