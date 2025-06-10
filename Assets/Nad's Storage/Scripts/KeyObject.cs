using UnityEngine;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour
{
    [Tooltip("How far above the player the key should float once collected")]
    [Header("Position Offset")]
    public Vector3 localOffset = new Vector3(0f, 2f, 0f);
    [Header("Player or Bird Key")]
    public bool isPlayerKey = true; 

    public UnityEvent onKeyCollected;

    private bool _collected = false;

    public void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (isPlayerKey && other.CompareTag("Player"))
        {
            _collected = true;
        } // only react to the player if it's a player key
        else if (!isPlayerKey && other.CompareTag("Bird"))
        {
            _collected = true;
        } // only react to the bird if it's a bird key
        else
        {
            return; // ignore other objects
        }

        onKeyCollected.Invoke();

        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        Transform playerT = other.transform;
        transform.SetParent(playerT, worldPositionStays: false);

        transform.localPosition = localOffset;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void keyDropped()
    {
        
    }
}
