using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [Header("Scene Transition")]
    public string targetSceneName;
    public string transitionSceneName;
    public bool requiresKey = false;

    [Header("Trigger Type")]
    public bool useCollisionTrigger = true;
    public bool useKeyPress = false;
    public KeyCode activationKey = KeyCode.E;

    [Header("Visual Feedback")]
    public GameObject promptUI;

    private bool playerInRange = false;
    private PlayerController player;

    void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        if (useKeyPress && playerInRange && Input.GetKeyDown(activationKey))
        {
            TriggerTransition();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            playerInRange = true;

            if (promptUI != null)
                promptUI.SetActive(true);

            if (useCollisionTrigger)
            {
                TriggerTransition();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }

    private void TriggerTransition()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Target scene name is not set!");
            return;
        }

        if (requiresKey && player != null && !player.gotKey)
        {
            Debug.Log("Player needs a key to access this area!");
            return;
        }

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.StartTransition(targetSceneName, transitionSceneName);
        }
        else
        {
            Debug.LogError("TransitionManager not found! Make sure it exists in the scene.");
        }
    }
}