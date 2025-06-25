using UnityEngine;
using UnityEngine.UI;

public class TransitionButton : MonoBehaviour
{
    [Header("Scene Transition")]
    public string targetSceneName;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogWarning("TransitionButton script requires a Button component!");
        }
    }

    public void OnButtonClick()
    {
        StartTransition();
    }

    public void StartTransition()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Target scene name is not set!");
            return;
        }

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.StartTransition(targetSceneName);
        }
        else
        {
            Debug.LogError("TransitionManager not found! Make sure it exists in the scene.");
        }
    }
}