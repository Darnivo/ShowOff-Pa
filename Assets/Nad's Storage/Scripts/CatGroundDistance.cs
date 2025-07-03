using UnityEngine;

public class CatGroundDistance : MonoBehaviour
{
    private CatAnimation catAnimation;
    private Collider currentGround;
    void Awake()
    {
        catAnimation = GetComponentInParent<CatAnimation>();
        catAnimation.isLanding(true);
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & catAnimation.groundLayers) != 0)

        {
            catAnimation.isLanding(true);
            currentGround = collision;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & catAnimation.groundLayers) != 0)

        {
            catAnimation.isLanding(false);
        }
    }

    private void Update()
    {
        if (currentGround != null)
        {
            if (!currentGround.gameObject.activeInHierarchy)
            {
                currentGround = null;
                catAnimation.isLanding(false); 
            }
        }
    }
}