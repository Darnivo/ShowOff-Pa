using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag your Player GameObject here")]
    public Transform player;

    [Tooltip("How quickly the camera catches up to the player")]
    public float smoothSpeed = 0.125f;

    // The offset from the player at start
    private Vector3 offset;

    void Start()
    {
        // calculate initial offset based on current positions
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // desired position is player's position plus our initial offset
        // Vector3 desiredPosition = player.position + offset;

        // for a strict side-scroller you might only want to follow X (and maybe Y):
        Vector3 desiredPosition = new Vector3(player.position.x + offset.x,
                                      player.position.y + offset.y,
                                      transform.position.z);

        // smooth the movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);


        transform.position = smoothedPosition;
    }
}
