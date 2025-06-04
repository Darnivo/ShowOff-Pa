using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetA;
    public Transform targetB;
    public float smoothSpeed = 0.125f;
    public float zoomLimiter = 10f;
    public float minZoom = 40f;
    public float maxZoom = 60f;

    private Vector3 velocity;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraFollow must be attached to a Camera!");
        }
    }

    void LateUpdate()
    {
        if (targetA == null || targetB == null) return;

        Vector3 centerPoint = (targetA.position + targetB.position) / 2f;
        Vector3 desiredPosition = new Vector3(centerPoint.x, centerPoint.y, transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);

        float distance = Vector3.Distance(targetA.position, targetB.position);
        float zoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.fieldOfView = Mathf.Clamp(zoom, minZoom, maxZoom);

    }
}
