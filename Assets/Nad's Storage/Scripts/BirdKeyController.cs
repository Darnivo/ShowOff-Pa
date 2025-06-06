using UnityEngine;

public class BirdKeyController : MonoBehaviour
{
    public bool gotKey = false;
    public void PickUpKey()
    {
        gotKey = true;
    }
    public void DropKey()
    {
        gotKey = false; 
    }
}