using UnityEngine;

public class DreamObject : MonoBehaviour
{
    private MeshRenderer meshRendererComponent;
    private Collider colliderComponent;
    private bool isRevealed = false;

    void Awake()
    {
        meshRendererComponent = GetComponent<MeshRenderer>();
        colliderComponent = GetComponent<Collider>(); // Use Collider2D for 2D projects

        // Ensure components exist
        if (meshRendererComponent == null)
        {
            Debug.LogError("DreamObject " + gameObject.name + " is missing a MeshRenderer component.");
        }
        if (colliderComponent == null)
        {
            // For 2D, this would be Collider2D
            Debug.LogError("DreamObject " + gameObject.name + " is missing a Collider component.");
        }
    }

    void Start()
    {
        // Start with components disabled
        SetComponentsEnabled(false);
    }

    public void Reveal()
    {
        if (!isRevealed)
        {
            isRevealed = true;
            SetComponentsEnabled(true);
            Debug.Log(gameObject.name + " has been revealed.");
            // Optional: You might want to disable this script or the GameObject
            // after revealing if it serves no other purpose.
            // this.enabled = false;
        }
    }
    public void Hide()
    {
        if (isRevealed)
        {
            isRevealed = false;
            SetComponentsDisabled(true);
            Debug.Log(gameObject.name + " has been hidden.");
        }
    }

    private void SetComponentsEnabled(bool enabled)
    {
        if (meshRendererComponent != null)
        {
            meshRendererComponent.enabled = enabled;
        }


        if (colliderComponent != null)
        {
            colliderComponent.enabled = enabled;
        }
    }

    private void SetComponentsDisabled(bool disabled)
    {
        if (meshRendererComponent != null)
        {
            meshRendererComponent.enabled = !disabled;
        }

        if (colliderComponent != null)
        {
            colliderComponent.enabled = !disabled;
        }
    }

    // Public method to check if the object is revealed
    public bool IsRevealed()
    {
        return isRevealed;
    }

    // Public method to get the position of the object
    public Vector3 GetPosition()
    {
        return transform.position;
    }




}