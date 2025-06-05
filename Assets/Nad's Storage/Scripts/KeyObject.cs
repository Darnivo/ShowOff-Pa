using UnityEngine;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour
{
    [Tooltip("How far above the player the key should float once collected")]
    public Vector3 localOffset = new Vector3(0f, 2f, 0f);

    public UnityEvent onKeyCollected;

    private bool _collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player"))   // only react to the player
            return;

        _collected = true;

        //Invoke any callbacks you have (so PlayerController can set its gotKey flag, play sound, etc.)
        onKeyCollected.Invoke();

        //Disable this collider so it can't be retriggered
        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        //Re-parent the key under the player and move it up by localOffset
        Transform playerT = other.transform;
        transform.SetParent(playerT, worldPositionStays: false);

        // Because worldPositionStays = false, the key's localPosition is currently (0,0,0).
        // Now just shift it up by localOffset:
        transform.localPosition = localOffset;

        // If the key has its own Rigidbody, you might want to freeze or disable it
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
