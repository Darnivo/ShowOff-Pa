using UnityEngine;
using UnityEngine.Events;

public class GameEvent : MonoBehaviour
{
    UnityEvent eventToTrigger; 

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            eventToTrigger.Invoke();
        }
    }
}
