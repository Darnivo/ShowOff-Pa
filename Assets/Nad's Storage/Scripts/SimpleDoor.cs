using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public float openHeight = 3f;
    public float openSpeed = 2f;
    private Vector3 closedPos;
    private Vector3 openedPos;
    private bool keyDelivered = false; 
    void Start()
    {
        closedPos = transform.position; 
        openedPos = transform.position + new Vector3(0, openHeight, 0);
    }

    void Update()
    {
        if (keyDelivered == true)
        {
            OpenDoor(); 
        }
    }
    public void keyToggle()
    {
        keyDelivered = true;
    }

    public void OpenDoor()
    {
        transform.position = Vector3.MoveTowards(transform.position, openedPos, openSpeed * Time.deltaTime);
    }
}