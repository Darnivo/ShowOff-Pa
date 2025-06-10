using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [Header("Targets & Follow")]
    public Transform targetA;
    public Transform targetB;
    public float smoothSpeed = 0.125f;

    [Header("Zoom")]
    public float zoomLimiter = 10f;
    public float minZoom = 40f;
    public float maxZoom = 60f;

    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeRoutine;


    private Vector3 velocity;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            Debug.LogError("CameraFollow must be on a Camera!");
    }

    void LateUpdate()
    {
        if (targetA == null || targetB == null) return;
        Vector3 centerPoint = (targetA.position + targetB.position) * 0.5f;
        Vector3 desiredPos = new Vector3(centerPoint.x, centerPoint.y, transform.position.z);
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);

        float distance = Vector3.Distance(targetA.position, targetB.position);
        float fov = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);
        cam.fieldOfView = Mathf.Clamp(fov, minZoom, maxZoom);

        transform.position = smoothedPos + shakeOffset;

        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     Shake(0.1f, 0.1f);
        // }
    }
    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * magnitude;
            offset.z = 0f;
            shakeOffset = offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}

/* How to use:
find the CameraFollow (cache this reference in Awake for performance!)
var camFollow = Camera.main.GetComponent<CameraFollow>();

shake for .3 seconds, with up to 0.5 units
camFollow.Shake(0.3f, 0.5f);
*/