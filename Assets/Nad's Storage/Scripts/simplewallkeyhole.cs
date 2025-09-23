using UnityEngine;
using System.Collections;

public class theWall : MonoBehaviour
{
    public float disappearDelay = 2f;
    public void onKeyDelivered()
    {
        StartCoroutine(disableDoor());
    }
    private IEnumerator disableDoor()
    {
        yield return new WaitForSeconds(disappearDelay);
        gameObject.SetActive(false);
    }
}