using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign in inspector
    private bool isPaused = false;
    public List<MoveClouds> moveClouds = new List<MoveClouds>();

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                moveCloudsBack();
                ResumeGame();
                  
            }
            else
            {
                PauseGame();
                moveCloudsIn();
            }
        }
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
        if(moveClouds.Count == 0)
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
        if(moveClouds.Count == 0)
        {
            Debug.LogWarning("No clouds found to move back.");
            return;
        }
        foreach (MoveClouds cloud in moveClouds)
        {
            cloud.moveBack(); 
        }
    }
}