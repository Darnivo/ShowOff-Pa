using UnityEngine;
using UnityEngine.SceneManagement;

public class menutimer : MonoBehaviour
{
    [Header("Transition Settings")]
    public float creditsDuration = 30f; // How long credits run
    public string menuSceneName = "Title Screen"; // Name of your menu scene
    
    void Start()
    {
        // Start countdown to return to menu
        Invoke("ReturnToMenu", creditsDuration);
    }
    
    void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
    
    void Update()
    {
        // Optional: Allow player to skip credits
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke("ReturnToMenu"); // Cancel the timer
            ReturnToMenu();
        }
    }
}