using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in inspector
    public GameObject buttonList;
    public Button pauseButton; // Add this - assign in inspector
    [Header("The Pages")]
    public GameObject optionsUI; 
    public GameObject mainUI;
    
    private bool isPaused = false;
    [HideInInspector]
    public List<MoveClouds> moveClouds = new List<MoveClouds>();
    private List<Button> buttons = new List<Button>();

    void Awake()
    {
        // Your existing Awake code...
        Transform[] children = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.TryGetComponent<MoveClouds>(out MoveClouds moveCloud))
            {
                moveClouds.Add(moveCloud);
            }
        }
        Transform[] buttonChildren = buttonList.GetComponentsInChildren<Transform>(true);
        foreach (Transform buttonChild in buttonChildren)
        {
            if (buttonChild.TryGetComponent<Button>(out Button button))
            {
                buttons.Add(button);
            }
        }
        disableButtons();
        pauseMenuUI.SetActive(false);
        
        // Set up the pause button
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(OnPauseButtonClick);
        }
    }

    void Update()
    {
        // Keep your existing Escape key functionality
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                moveCloudsBack();
                disableButtons();
                ResumeGame();
            }
            else
            {
                PauseGame();
                moveCloudsIn();
                disableButtons();
            }
        }
        checkClouds();
    }

    // New method for button click
    public void OnPauseButtonClick()
    {
        if (!isPaused)
        {
            PauseGame();
            moveCloudsIn();
            disableButtons();
        }
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        
        // Hide the pause button when paused
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {

        moveCloudsBack();

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Show the pause button when resumed
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
        optionsUI.SetActive(false);
        mainUI.SetActive(true); 
        
    }

    // Rest of your existing methods remain the same...
    private void moveCloudsIn()
    {
        if (moveClouds.Count == 0)
        {
            Debug.LogWarning("No clouds found to move in.");
            return;
        }
        foreach (MoveClouds cloud in moveClouds)
        {
            cloud.triggerMove();
        }
    }

    private void moveCloudsBack()
    {
        if (moveClouds.Count == 0)
        {
            Debug.LogWarning("No clouds found to move back.");
            return;
        }
        foreach (MoveClouds cloud in moveClouds)
        {
            cloud.moveBack();
        }
    }

    private void enableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
    
    private void disableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    private void checkClouds()
    {
        foreach (MoveClouds cloud in moveClouds)
        {
            if (!cloud.isMoving)
            {
                enableButtons();
            }
        }
    }
    
    public void RestartGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        moveCloudsBack();
        disableButtons();
        ResumeGame();
    }
}