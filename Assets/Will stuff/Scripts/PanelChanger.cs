using UnityEngine;

public class PanelChanger : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

