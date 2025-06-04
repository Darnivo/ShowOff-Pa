using UnityEngine;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    public UnityEvent keyDelivered;
    public PlayerController player;
    private bool gotkey; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.gotKey == true)
            {
                // Debug.Log("Key has been delivered");
                keyDelivered.Invoke();
            }
            else
            {
                Debug.Log("Player doesn't have key"); 
            }
        }
    }


}