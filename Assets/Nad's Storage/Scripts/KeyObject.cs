using UnityEngine;
using UnityEngine.Events;

public class KeyObject : MonoBehaviour
{
    [Tooltip("How far above the player the key should float once collected")]
    [Header("Position Offset")]
    public Vector3 localOffset = new Vector3(0f, 2f, 0f);
    [Header("Player or Bird Key")]
    public bool isPlayerKey = true;
    private PlayerController player;
    private BirdKeyController bird;

    public UnityEvent onKeyCollected;

    private bool _collected = false;
    private Vector3 _originalPosition;

    public void Start()
    {
        _originalPosition = transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (isPlayerKey && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            _collected = true;
            player.OnKeyCollected(); 
            onKeyCollected.Invoke(); 
        } // only react to the player if it's a player key
        else if (!isPlayerKey && other.CompareTag("Bird"))
        {
            BirdKeyController bird = other.GetComponent<BirdKeyController>();
            _collected = true;
            bird.PickUpKey(); 
        } // only react to the bird if it's a bird key
        else
        {
            return; // ignore other objects
        }


        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;

        Transform playerT = other.transform;
        transform.SetParent(playerT, worldPositionStays: false);

        if (isPlayerKey)
        {
            transform.localPosition = localOffset;
        }
        if (!isPlayerKey)
            {
                transform.localScale = Vector3.one * 0.3f; // Scale down the bird key
                transform.localPosition = new Vector3(localOffset.x, localOffset.y-1.1f, localOffset.z); // Adjust 
            }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void keyDropped()
    {
        transform.SetParent(null, worldPositionStays: true);
        transform.position = _originalPosition; // Reset position to original
        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = true;
        if (player != null)
        {
            player.OnKeyLost(); 
        }
        if( bird != null)
        {
            bird.DropKey(); 
        }
        _collected = false; // Reset collected state
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero; // Reset velocity
        }
    }
}
