using UnityEngine;

public class CatGroundDistance : MonoBehaviour
{
    private CatAnimation catAnimation;
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
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (((1 << collision.gameObject.layer) & catAnimation.groundLayers) != 0)

        {
            catAnimation.isLanding(false);
        }
    }
}