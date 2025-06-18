using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class DreamObjectManager : MonoBehaviour
{
    private List<GameObject> dreamObjects = new List<GameObject>(); // the actual obstacles 
    private List<GameObject> dreamPlaceHolder = new List<GameObject>(); // the 2D drawing marker things, when it is inactive
    private DreamObjectCollider dreamCollider; 
    private DreamObjectState dreamObjectState = DreamObjectState.INACTIVE;
    private List<DissolveEffect> dissolveManager = new List<DissolveEffect>(); // the dissolve effect manager for the dream objects
    public float disappearDelay = 1f;
    private DreamObjectState prevState;
    [Header("Dissolve Settings")]
    public float dissolveDuration = 0.5f; // duration for the dissolve effect


    

    void Start()
    {
        prevState = dreamObjectState;
        setArray();
        // updateState();
        StartCoroutine(disableDreamObjects());
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
            // foreach (var dis in dissolveManager)
            // {
            //     dis.dissolveOut(dissolveDuration);
            // }
            DisolveOutAll();
            StartCoroutine(disableDreamObjects());
        }
        else
        {
            foreach (GameObject obj in dreamObjects)
            {
                if (obj.CompareTag("DreamObject"))
                {
                    obj.SetActive(true);
                    // dissolveManager.Find(d => d.gameObject == obj).dissolveIn(dissolveDuration);
                }
            }
            DisolveInAll(); 
        }
    }

    private void DisolveOutAll()
    {
        foreach (var dis in dissolveManager)
        {
            dis.dissolveOut(dissolveDuration);
        }
    }

    private void DisolveInAll()
    {
        foreach (var dis in dissolveManager)
        {
            dis.dissolveIn(dissolveDuration);
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
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("DreamObject"))
            {
                dreamObjects.Add(child.gameObject);
                DissolveEffect dissolveEffect = child.GetComponent<DissolveEffect>();
                if (dissolveEffect == null)
                {
                    Transform[] grandchild = child.GetComponentsInChildren<Transform>(true);
                    foreach (Transform gchild in grandchild)
                    {
                        dissolveEffect = gchild.GetComponent<DissolveEffect>();
                        if(dissolveEffect != null)
                        {
                            dissolveManager.Add(dissolveEffect);
                        }
                    }
                    // dissolveEffect = child.GetComponentInChildren<DissolveEffect>();
                }
                dissolveManager.Add(dissolveEffect);
                if (dissolveManager[dissolveManager.Count - 1] == null)
                {
                    Debug.LogError("no dissolve script in: " + child.name);
                }
                child.gameObject.SetActive(false);
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
