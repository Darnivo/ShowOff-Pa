using UnityEngine;

public class Fan : MonoBehaviour
{
    public float pushForce = 10f;
    public Vector3 pushDirection = Vector3.forward;

    private GameObject dreamObject;
    private bool wasSoundPlaying = false;

    private void Start()
    {
        dreamObject = transform.Find("default")?.gameObject;

        if (dreamObject == null)
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.CompareTag("DreamObject"))
                {
                    dreamObject = child.gameObject;
                    break;
                }
            }
        }

        if (dreamObject == null)
        {
            Debug.LogWarning("Fan: Could not find dream object child!");
        }
        else
        {
            bool isVisible = dreamObject.activeInHierarchy;
            if (!isVisible && SFXManager.Instance != null)
            {
                SFXManager.Instance.StopFanSound();
                wasSoundPlaying = false;
            }
            else if (isVisible)
            {
                wasSoundPlaying = true;
            }
        }
    }

    private void Update()
    {
        if (dreamObject != null)
        {
            bool isVisible = dreamObject.activeInHierarchy;

            if (isVisible && !wasSoundPlaying)
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayFanSound();
                }
                wasSoundPlaying = true;
            }
            else if (!isVisible && wasSoundPlaying)
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.StopFanSound();
                }
                wasSoundPlaying = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (dreamObject != null && !dreamObject.activeInHierarchy)
            return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !rb.isKinematic)
        {
            rb.AddForce(pushDirection.normalized * pushForce, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.rotation * pushDirection.normalized * 2f);
    }
}