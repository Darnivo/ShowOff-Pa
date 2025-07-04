using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    public string defaultTransitionSceneName = "TransitionScene";
    public float minTransitionTime = 2f;
    public float maxTransitionTime = 5f;

    private string targetSceneName;
    private string currentTransitionSceneName;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartTransition(string targetScene, string transitionScene = null)
    {
        if (isTransitioning) return;

        targetSceneName = targetScene;
        currentTransitionSceneName = string.IsNullOrEmpty(transitionScene) ? defaultTransitionSceneName : transitionScene;
        isTransitioning = true;

        Debug.Log($"Starting transition: {currentTransitionSceneName} -> {targetSceneName}");
        SceneManager.LoadScene(currentTransitionSceneName);
    }


    public void StartTransition(string sceneName)
    {
        StartTransition(sceneName, defaultTransitionSceneName);
    }

    public void CompleteTransition()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"Completing transition to: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
            targetSceneName = "";
            currentTransitionSceneName = "";
            isTransitioning = false;
        }
    }

    public float GetRandomTransitionTime()
    {
        return Random.Range(minTransitionTime, maxTransitionTime);
    }

    public string GetCurrentTransitionScene()
    {
        return currentTransitionSceneName;
    }
}