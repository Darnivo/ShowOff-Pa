using UnityEngine;
using UnityEngine.UI;

public class MoveClouds : MonoBehaviour
{
    public Vector2 offset;
    public float speed = 50f;
    private RectTransform cloud;
    public bool isMoving = false;
    private Vector2 initialPosition;
    void Awake()
    {
        cloud = GetComponent<RectTransform>();
        initialPosition = cloud.anchoredPosition; // Store the initial position 
    }

    void Update()
    {
        if (isMoving)
        {
            moveToPosition();
        }
    }

    public void triggerMove()
    {
        isMoving = true;
    }


    private void moveToPosition()
    {
        cloud.anchoredPosition = Vector2.MoveTowards(
            cloud.anchoredPosition,
            initialPosition + offset,
            speed * Time.unscaledDeltaTime
        );
        if (Vector2.Distance(cloud.anchoredPosition, initialPosition + offset) < 0.1f)
        {
            cloud.anchoredPosition = initialPosition + offset;
            isMoving = false;
        }
    }

    public void moveBack()
    {
        isMoving = false;
        cloud.anchoredPosition = initialPosition;

    }

}