using UnityEngine;

public class Spin : MonoBehaviour
{

    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // degrees per second

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
