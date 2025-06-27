using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class kill : MonoBehaviour
{
    public UnityEvent PlayerDeath;
    public List<KeyObject> keys; 
    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDeathHandler>(out var deathHandler))
        {
            deathHandler.onDeath();
            if (other.CompareTag("Player"))
            {
                foreach (var key in keys)
                {
                    key.keyDropped();
                    key.playerDied();
                }
            }
            
        }
        if (other.CompareTag("Player"))
        {
            PlayerDeath.Invoke();
        }

    } 

    

}
