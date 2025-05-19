using UnityEngine;

public class DreamObjectManager : MonoBehaviour
{
    private GameObject[] dreamObjects; 

    void Start()
    {
        dreamObjects = GetComponentsInChildren<GameObject>(true);
    }

    public GameObject[] getDreamObjects()
    {
        return dreamObjects;
    }
}
