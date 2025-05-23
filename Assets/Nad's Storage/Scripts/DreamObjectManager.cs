using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DreamObjectManager : MonoBehaviour
{
    private List<GameObject> dreamObjects = new List<GameObject>(); // the actual obstacles 
    private List<GameObject> dreamMarker = new List<GameObject>(); // the 2D drawing marker things, when it is inactive
    private DreamObjectCollider dreamCollider; 
    private DreamObjectState dreamObjectState = DreamObjectState.INACTIVE;
    public float disappearDelay = 0.5f;
    private DreamObjectState prevState; 

    void Start()
    {
        prevState = dreamObjectState;
        setArray();
        updateState();
        resizeCollider();
        
    }

    void Update()
    {
        // if(prevState != dreamObjectState)
        // {
        //     prevState = dreamObjectState;
        //     updateState();
        // }
        updateState();
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
        // StopAllCoroutines();
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
            }
            else if (child.CompareTag("DreamObjectCollider"))
            {
                dreamCollider = child.GetComponent<DreamObjectCollider>();
            }
        }
    }


    public enum DreamObjectState
    {
        ACTIVE,
        INACTIVE
    }

}
