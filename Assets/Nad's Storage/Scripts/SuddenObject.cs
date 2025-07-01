using UnityEngine;
using UnityEngine.Events; 

public class SuddenObject : MonoBehaviour
{
    [Header("Obstacle")]
    public GameObject obstacle;
    public UnityEvent onPlayerEnter; 

    void Awake()
    {
        if(obstacle!= null) obstacle.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        onPlayerEnter.Invoke(); 
    }

}