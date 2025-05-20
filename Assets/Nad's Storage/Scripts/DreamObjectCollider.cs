using UnityEngine;

public class DreamObjectCollider : MonoBehaviour
{
    private DreamObjectManager dreamObjectManager;

    void Start()
    {
        dreamObjectManager = GetComponentInParent<DreamObjectManager>();
        if (dreamObjectManager == null)
        {
            Debug.LogError("DreamObjectManager not found in the scene.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            Debug.Log("Sight Triggered");
            dreamObjectManager.SetToActive();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            Debug.Log("Sight Triggered");
            dreamObjectManager.SetToInactive();
        }
    }

    public void resizeSelf(Renderer[] renderers)
    {
        if (renderers.Length == 0)
        {
            Debug.LogError("No renderers found to resize the collider.");
            return;
        }
        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        transform.position = bounds.center;
        transform.localScale = bounds.size;


    }

}