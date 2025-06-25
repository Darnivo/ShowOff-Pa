using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    public string transitionSceneName = "TransitionScene";
    public float minTransitionTime = 2f;
    public float maxTransitionTime = 5f;

    private string targetSceneName;
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

    public void StartTransition(string sceneName)
    {
        if (isTransitioning) return;

        targetSceneName = sceneName;
        isTransitioning = true;

        SceneManager.LoadScene(transitionSceneName);
    }

    public void CompleteTransition()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
            targetSceneName = "";
            isTransitioning = false;
        }
    }

    public float GetRandomTransitionTime()
    {
        return Random.Range(minTransitionTime, maxTransitionTime);
    }
}