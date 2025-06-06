using UnityEngine;
using UnityEngine.Events; 

public class BirdKey : MonoBehaviour
{
    public UnityEvent onBirdKey; 
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Bird"))
        {
            onBirdKey.Invoke(); 
        }
    }
}