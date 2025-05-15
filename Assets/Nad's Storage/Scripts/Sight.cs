using UnityEngine;

public class SightBox : MonoBehaviour
{
    public Transform player; 
    private Camera mainCamera; 
    private bool _followingPlayer = false; 

    void Start()
    {
        mainCamera = Camera.main; 
    }
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.P)){
            _followingPlayer = !_followingPlayer;
        }
        SightFollowSettings();
        
    }
    
    private void SightFollowSettings(){
        if(_followingPlayer && player != null)
        {
            transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);

        }
        else
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(mainCamera.transform.position.z);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

        }
    }

}
