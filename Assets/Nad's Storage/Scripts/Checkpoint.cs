using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;
    public Material activatedMaterial; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.SetRespawnPoint(transform);
            isActivated = true;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = activatedMaterial; 
        }
    }
}