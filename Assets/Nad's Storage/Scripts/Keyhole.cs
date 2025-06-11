using UnityEngine;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    public UnityEvent keyDelivered;
    public keyType neededKey; 
    private PlayerController player;
    private BirdKeyController bird;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && neededKey == keyType.PLAYERKEY)
        {
            player = other.GetComponent<PlayerController>();
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
        else if (other.CompareTag("Bird") && neededKey == keyType.BIRDKEY)
        {
            bird = other.GetComponent<BirdKeyController>();
            if (bird.gotKey == true)
            {
                keyDelivered.Invoke();
            }
            else
            {
                Debug.Log("Bird doesn't have key");
            }
        }
    }
    public enum keyType
    {
        PLAYERKEY,
        BIRDKEY
    }


}