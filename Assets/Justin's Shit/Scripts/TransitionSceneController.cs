using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TransitionSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI loadingText;
    public Slider progressBar;
    public Image backgroundImage;

    [Header("Loading Messages")]
    public string[] loadingMessages = {
        "Loading...",
        "Preparing adventure...",
        "Gathering dreams...",
        "Almost there...",
        "Just a moment..."
    };

    [Header("Visual Effects")]
    public float textBlinkSpeed = 1f;
    public Color[] backgroundColors;

    private float transitionDuration;
    private float currentTime = 0f;

    void Start()
    {
        if (TransitionManager.Instance != null)
        {
            transitionDuration = TransitionManager.Instance.GetRandomTransitionTime();
        }
        else
        {
            transitionDuration = 3f;
        }

        SetRandomLoadingMessage();
        SetRandomBackgroundColor();
        StartCoroutine(TransitionSequence());
    }

    private IEnumerator TransitionSequence()
    {
        while (currentTime < transitionDuration)
        {
            currentTime += Time.deltaTime;

            UpdateProgressBar();
            UpdateLoadingText();

            if (Random.Range(0f, 1f) < 0.1f * Time.deltaTime)
            {
                SetRandomLoadingMessage();
            }

            yield return null;
        }

        CompleteTransition();
    }

    private void UpdateProgressBar()
    {
        if (progressBar != null)
        {
            float progress = currentTime / transitionDuration;
            progressBar.value = Mathf.Lerp(0f, 1f, progress);
        }
    }

    private void UpdateLoadingText()
    {
        if (loadingText != null)
        {
            float alpha = Mathf.PingPong(Time.time * textBlinkSpeed, 1f);
            Color textColor = loadingText.color;
            textColor.a = Mathf.Lerp(0.3f, 1f, alpha);
            loadingText.color = textColor;
        }
    }

    private void SetRandomLoadingMessage()
    {
        if (loadingText != null && loadingMessages.Length > 0)
        {
            int randomIndex = Random.Range(0, loadingMessages.Length);
            loadingText.text = loadingMessages[randomIndex];
        }
    }

    private void SetRandomBackgroundColor()
    {
        if (backgroundImage != null && backgroundColors.Length > 0)
        {
            int randomIndex = Random.Range(0, backgroundColors.Length);
            backgroundImage.color = backgroundColors[randomIndex];
        }
    }

    private void CompleteTransition()
    {
        if (progressBar != null)
            progressBar.value = 1f;

        if (loadingText != null)
            loadingText.text = "Complete!";

        Invoke(nameof(FinishTransition), 0.5f);
    }

    private void FinishTransition()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.CompleteTransition();
        }
        else
        {
            Debug.LogError("TransitionManager not found!");
        }
    }
}