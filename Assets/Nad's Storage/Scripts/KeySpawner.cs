using UnityEngine;
using System.Collections.Generic;

public class KeySpawner : MonoBehaviour
{
    public GameObject keyPrefab;
    public void KeyObtained()
    {
        keyPrefab.SetActive(false);
    }
    public void KeyReturned()
    {
        keyPrefab.SetActive(true);
    }


}