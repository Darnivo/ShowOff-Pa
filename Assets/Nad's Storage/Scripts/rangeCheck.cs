using UnityEngine;

public class RangeCheck : MonoBehaviour
{
    public LayerMask stickyWallMask; // Layer mask to define what is considered a sticky wall
    [HideInInspector]
    public bool wallWithinRange = false;
    private CatAnimation catAnimation;
    void Start()
    {
        catAnimation = GetComponentInParent<CatAnimation>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((stickyWallMask.value & (1 << other.gameObject.layer)) != 0)
        {
            wallWithinRange = true;
            catAnimation.wallRange(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((stickyWallMask.value & (1 << other.gameObject.layer)) != 0)
        {
            wallWithinRange = false;
            catAnimation.wallRange(false);
            
        }
    }
}