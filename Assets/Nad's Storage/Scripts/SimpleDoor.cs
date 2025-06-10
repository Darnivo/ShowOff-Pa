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
        else if (keyDelivered == false)
        {
            CloseDoor();
        }
    }
    public void keyToggleTrue()
    {
        keyDelivered = true;
    }

    public void keyToggleFalse()
    {
        keyDelivered = false;
    }

    public void OpenDoor()
    {
        transform.position = Vector3.MoveTowards(transform.position, openedPos, openSpeed * Time.deltaTime);
    }

    public void CloseDoor()
    {
        transform.position = Vector3.MoveTowards(transform.position, closedPos, openSpeed * Time.deltaTime);
    }
}