using UnityEngine;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour
{
    public UnityEvent onKeyCollected;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onKeyCollected.Invoke();
        }
    }


    
}
