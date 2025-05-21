using System.Transactions;
using UnityEngine;

public class paralax : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float length, startpos;
    public GameObject cam;
    public float paralaxEffect;

    void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = (cam.transform.position.x * paralaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
    }
}
