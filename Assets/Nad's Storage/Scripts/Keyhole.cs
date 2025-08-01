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
    public UnityEvent duringDelay;
    private bool activated = false; 

    void OnTriggerEnter(Collider other)
    {
        if (activated == true) return; 
        if (other.CompareTag("Player") && neededKey == keyType.PLAYERKEY)
        {
            player = other.GetComponent<PlayerController>();
            if (player.gotKey == true)
            {
                // Debug.Log("Key has been delivered");
                keyDelivered.Invoke();
                activated = true; 
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
        duringDelay.Invoke(); 
        yield return new WaitForSeconds(delayTime);
        if (isInSight)
        {
            keyDelivered.Invoke();
            activated = true; 
        }

    }
    public enum keyType
    {
        PLAYERKEY,
        BIRDKEY
    }


}