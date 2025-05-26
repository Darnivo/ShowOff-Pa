using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DreamObjectManager : MonoBehaviour
{
    private List<GameObject> dreamObjects = new List<GameObject>(); // the actual obstacles 
    private List<GameObject> dreamPlaceHolder = new List<GameObject>(); // the 2D drawing marker things, when it is inactive
    private DreamObjectCollider dreamCollider; 
    private DreamObjectState dreamObjectState = DreamObjectState.INACTIVE; 
    public float disappearDelay = 1f;
    private DreamObjectState prevState;
    [Header("Flicker Settings")]
    public float flickerInterval = 0.01f;
    public float flickerDuration = 0.02f; 
    private MeshRenderer dreamObjectRenderer;
    

    void Start()
    {
        prevState = dreamObjectState;
        setArray();
        updateState();
        resizeCollider();
        
    }

    void Update()
    {
        resizeCollider();
        if (prevState != dreamObjectState)
        {
            prevState = dreamObjectState;
            updateState();
        }
        // updateState();
    }

    public void SetToActive()
    {
        dreamObjectState = DreamObjectState.ACTIVE;
    }
    public void SetToInactive()
    {
        dreamObjectState = DreamObjectState.INACTIVE;
    }

    private void updateState()
    {
        StopAllCoroutines();
        if (dreamObjectState == DreamObjectState.INACTIVE)
        {
            StartCoroutine(disableDreamObjects());
        }
        else
        {
            foreach (GameObject obj in dreamObjects)
            {
                if (obj.CompareTag("DreamObject"))
                {
                    obj.SetActive(true);
                }
            }

        }
    }
    
    private IEnumerator disableDreamObjects()
    {
        yield return new WaitForSeconds(disappearDelay);
        foreach (GameObject obj in dreamObjects)
        {
            if (obj.CompareTag("DreamObject"))
            {
                obj.SetActive(false);
            }
        }
    }

    private void resizeCollider()
    {
        List<Renderer> renderers = new List<Renderer>();
        foreach (GameObject obj in dreamObjects)
        {
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>(true));
        }

        dreamCollider.resizeSelf(renderers.ToArray());
    }



    private void setArray()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("DreamObject"))
            {
                dreamObjects.Add(child.gameObject);
                dreamObjectRenderer = child.GetComponent<MeshRenderer>();
                if (dreamObjectRenderer == null)
                {
                    Debug.LogWarning("DreamObject " + child.name + " is missing a MeshRenderer component.");
                }
                child.gameObject.SetActive(false); 
            }
            else if (child.CompareTag("DreamObjectCollider"))
            {
                dreamCollider = child.GetComponent<DreamObjectCollider>();
            }
        }
    }

    private IEnumerator flickerDreamObject()
    {
        float elapsedTime = 0f;
        while (elapsedTime < flickerDuration)
        {
            // Debug.Log("Flickering Dream Object");
            dreamObjectRenderer.enabled = !dreamObjectRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }
        dreamObjectRenderer.enabled = true; 
    }
    


    public enum DreamObjectState
    {
        ACTIVE,
        INACTIVE
    }

}
