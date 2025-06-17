using UnityEngine;

public class DreamObjMeshCollider : MonoBehaviour
{
    private DreamObjMesh dreamObjMesh;
    void Start()
    {
        dreamObjMesh = GetComponentInParent<DreamObjMesh>();
        if (dreamObjMesh == null)
        {
            Debug.LogError("DreamObjMesh not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            Debug.Log("Sight detected");
            dreamObjMesh.SetTo(1);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            Debug.Log("Sight exited");
            dreamObjMesh.SetTo(0);
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