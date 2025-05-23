using UnityEngine;
using UnityEngine.Events;

public class kill : MonoBehaviour
{
    public UnityEvent PlayerDeath;
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDeathHandler>(out var deathHandler)) {
            deathHandler.onDeath();
        }
        if (other.CompareTag("Player"))
        {
            PlayerDeath.Invoke();
        }

    } 

    

}
