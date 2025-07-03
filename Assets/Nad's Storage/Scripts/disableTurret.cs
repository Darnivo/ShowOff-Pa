using UnityEngine;
using UnityEngine.Events; 

public class DisableTurret : MonoBehaviour
{
    [Header("what to disable")]
    public GameObject turret;
    public UnityEvent onDisable; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            turret.SetActive(false);
            onDisable.Invoke(); 
        }
    }


}