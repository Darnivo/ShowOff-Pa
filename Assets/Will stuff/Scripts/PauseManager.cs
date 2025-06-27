using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in inspector
    public GameObject buttonList;
    private bool isPaused = false;
    [HideInInspector]
    public List<MoveClouds> moveClouds = new List<MoveClouds>();
    private List<Button> buttons = new List<Button>();

    void Awake()
    {
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
        disableButtons(); // Disable buttons initially
        pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden at start
    }

    void Update()
    {
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

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Optional: Disable player controls
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Optional: Re-enable player controls
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

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