using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitApplication()
    {
        Application.Quit(); 
        Debug.Log(">> Game has been stopped");
    }
}
