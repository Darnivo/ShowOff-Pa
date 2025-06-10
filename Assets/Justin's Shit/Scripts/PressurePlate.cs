using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Tooltip("Drag in your SimpleDoor here")]
    public SimpleDoor door;

    [Header("Plate Movement")]
    [Tooltip("How far down the plate sinks when pressed")]
    public float pressDepth = 0.1f;
    [Tooltip("How fast the plate moves up/down")]
    public float pressSpeed = 2f;

    private int occupants = 0;
    private Vector3 upPos;
    private Vector3 downPos;

    void Start()
    {
        upPos = transform.position;
        downPos = upPos + Vector3.down * pressDepth;
    }

    void Update()
    {
        Vector3 target = (occupants > 0) ? downPos : upPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            pressSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("DreamMarker"))
        {
            occupants++;
            if (occupants == 1)
                door.keyToggleTrue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("DreamMarker"))
        {
            occupants = Mathf.Max(0, occupants - 1);
            if (occupants == 0)
                door.keyToggleFalse();
        }
    }
}
