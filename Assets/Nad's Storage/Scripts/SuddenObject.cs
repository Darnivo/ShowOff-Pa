using UnityEngine;
using UnityEngine.Events; 

public class SuddenObject : MonoBehaviour
{
    [Header("Obstacle")]
    public GameObject obstacle;
    public UnityEvent onPlayerEnter;
    public bool dissolve = false;

    void Awake()
    {
        if (obstacle != null && dissolve == false) obstacle.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter.Invoke(); 
        }
        
    }

}