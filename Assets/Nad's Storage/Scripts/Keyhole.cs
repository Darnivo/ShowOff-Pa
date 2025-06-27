using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KeyHole : MonoBehaviour
{
    public UnityEvent keyDelivered;
    public keyType neededKey; 
    private PlayerController player;
    private BirdKeyController bird;
    [Header("Delay Settings")]
    public bool hasDelay = false;
    public float delayTime = 2f;
    private bool isInSight = false;
    private Coroutine lightUpCoroutine = null; 

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
            isInSight = true; 
            if (hasDelay)
            {
                StartCoroutine(lightUp());
            }
            else keyDelivered.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bird") && neededKey == keyType.BIRDKEY)
        {
            isInSight = false; 
        }
    }

    private IEnumerator lightUp()
    {
        yield return new WaitForSeconds(delayTime);
        if (isInSight)
        {
            keyDelivered.Invoke();
        }

    }
    public enum keyType
    {
        PLAYERKEY,
        BIRDKEY
    }


}