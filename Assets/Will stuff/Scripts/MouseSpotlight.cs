using UnityEngine;

public class MouseSpotlight : MonoBehaviour
{
    public Material spotlightMat; // Assign in Inspector
    private bool _followingPlayer = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            _followingPlayer = !_followingPlayer;
        }
        SightFollowSettings();
    }

    private void SightFollowSettings()
    {
        if (_followingPlayer)
        {
            Vector2 uv = new Vector2(0.5f, 0.5f);
           spotlightMat.SetVector("_MousePos", uv);

        }
        else
        {
            // Send mouse position in normalized screen UVs
            Vector2 mousePos = Input.mousePosition;
            Vector2 uv = new Vector2(mousePos.x / Screen.width, mousePos.y / Screen.height);
            spotlightMat.SetVector("_MousePos", uv);

        }
                    // Calculate and send screen aspect ratio (width / height)
            float aspect = (float)Screen.width / Screen.height;
            spotlightMat.SetFloat("_Aspect", aspect);

    }
}
