using System.Collections.Generic;
using System.Collections;
// using UnityEditor;
using UnityEngine;

public class DreamObjMesh : MonoBehaviour
{
    [Header("Dissolve/Disappear Settings")]
    public float disappearDelay = 1f; // delay before the object disappears
    public float dissolveDuration = 0.5f; // duration for the dissolve effect
    private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    private List<MeshCollider> meshColliders = new List<MeshCollider>();
    private List<DissolveEffect> dissolveEffects = new List<DissolveEffect>();
    private DreamObjMeshCollider dreamObjMesh; 
    private DreamObjectState dreamObjectState = DreamObjectState.INACTIVE;
    private DreamObjectState prevState;

    void Start()
    {
        prevState = dreamObjectState;
        setupArray(); 
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
    }

    private void updateState()
    {
        StopAllCoroutines();
        if (dreamObjectState == DreamObjectState.INACTIVE)
        {
            DissolveOutAll();
            StartCoroutine(disable());
        }
        else if (dreamObjectState == DreamObjectState.ACTIVE)
        {
            enable();
        }
    }

    private void DissolveOutAll()
    {
        foreach (var dissolveEffect in dissolveEffects)
        {
            dissolveEffect.dissolveOut(dissolveDuration);
        }
    }

    private void enable()
    {
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
        foreach (var meshCollider in meshColliders)
        {
            meshCollider.enabled = true;
        }
        foreach (var dissolveEffect in dissolveEffects)
        {
            dissolveEffect.dissolveIn(dissolveDuration);
        }
    }

    private IEnumerator disable()
    {
        yield return new WaitForSeconds(disappearDelay);
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
        foreach (var meshCollider in meshColliders)
        {
            meshCollider.enabled = false;
        }
    }
    private void resizeCollider()
    {
        List<Renderer> renderers = new List<Renderer>();
        foreach (var child in meshRenderers)
        {
            if (child.TryGetComponent<Renderer>(out var renderer))
            {
                renderers.Add(renderer);
            }
        }
        dreamObjMesh.resizeSelf(renderers.ToArray());
    }


    private void setupArray()
    {
        Transform[] currChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in currChildren)
        {
            if (child.CompareTag("DreamObject"))
            {
                Transform[] allGrandchild = child.GetComponentsInChildren<Transform>(true);
                foreach (var grandchild in allGrandchild)
                {
                    if (grandchild.TryGetComponent<MeshRenderer>(out var meshRenderer))
                    {
                        meshRenderers.Add(meshRenderer);
                    }
                    if (grandchild.TryGetComponent<MeshCollider>(out var meshCollider))
                    {
                        meshColliders.Add(meshCollider);
                    }
                    if (grandchild.TryGetComponent<DissolveEffect>(out var dissolveEffect))
                    {
                        dissolveEffects.Add(dissolveEffect);
                    }
                }
            }
            else if (child.CompareTag("DreamObjectCollider"))
            {
                dreamObjMesh = child.GetComponent<DreamObjMeshCollider>();
            }
        }
    }
    public void SetTo(int value)
    {
        if (value == 1)
        {
            dreamObjectState = DreamObjectState.ACTIVE;
        }
        else if (value == 0)
        {
            dreamObjectState = DreamObjectState.INACTIVE;
        }

    }
    public enum DreamObjectState
    {
        ACTIVE,
        INACTIVE
    }


}