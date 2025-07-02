using System;
using UnityEngine;

public class SpriteControl : MonoBehaviour
{
    private Transform spriteTransform;
    public bool isSight = false;
    public bool isNPC = false; 
    private Camera mainCam;
    private Rigidbody rb;

    private Vector3 sideLeft;
    private Vector3 sideRight;
    public Transform glowGroup;
    private CatAnimation cat_anim;
    [Header("For Bird")]
    public float flipThreshold = 0.1f;
    private SpriteRotate spriteRotation;


    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        if (spriteTransform == null && transform.childCount > 0)
        {
            spriteTransform = transform.GetChild(0);
        }
        sideLeft = spriteTransform.localScale;
        sideRight = spriteTransform.localScale;
        sideRight.x *= -1;
        if (!isNPC && !isSight)
        {
            cat_anim = GetComponent<CatAnimation>();
            spriteRotation = GetComponent<SpriteRotate>(); 
        }
        

    }

    void Update()
    {
        if (!isNPC)
        {
            checkDirection();
        }
    
    }
    private void LateUpdate()
    {
        if (glowGroup != null)
        {
            Vector3 glowScale = glowGroup.localScale;
            glowScale.x = spriteTransform.localScale.x >= 0 ? Mathf.Abs(glowScale.x) : -Mathf.Abs(glowScale.x);
            glowGroup.localScale = glowScale;
            Transform[] glowTransforms = glowGroup.GetComponentsInChildren<Transform>();
            foreach (Transform glowTransform in glowTransforms)
            {
                if (glowTransform == glowGroup) continue;
                Vector3 fixScale = glowTransform.localScale;
                // fixScale.x = spriteTransform.localScale.x >= 0 ? Mathf.Abs(fixScale.x) : -Mathf.Abs(fixScale.x);
                fixScale.x = Mathf.Abs(fixScale.x) * (spriteTransform.localScale.x >= 0 ? 1 : -1);
                glowTransform.localScale = fixScale;
            }
        }
    }

    private void checkDirection()
    {
        if (isSight)
        {
            // Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 screenMousePos = Input.mousePosition;
            screenMousePos.z = Mathf.Abs(mainCam.transform.position.z - transform.position.z); // or just bird’s Z

            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(screenMousePos);
            Vector3 targetPos = new Vector3(mouseWorldPos.x, transform.position.y, transform.position.z);

            if (Math.Abs(mouseWorldPos.x - transform.position.x) < flipThreshold)
            {
                return; // Do not flip if within threshold
            }
            if (mouseWorldPos.x > transform.position.x)
            {
                spriteTransform.localScale = sideRight;
            }
            else if (mouseWorldPos.x < transform.position.x)
            {
                spriteTransform.localScale = sideLeft;
            }

        }
        else
        {
            if(cat_anim != null && cat_anim.isOnStickyWall)
            {
                return; // Do not flip if on sticky wall
            }
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput > 0)
            {
                spriteTransform.localScale = sideLeft;
                spriteRotation.direction = 1;
            }
            else if (moveInput < 0)
            {
                spriteTransform.localScale = sideRight;
                spriteRotation.direction = -1;
            }

        }
    }

    public void flipNPC(Vector3 direction)
    {
        if (direction.x > 0.05f)
        {
            spriteTransform.localScale = sideLeft;
        }
        if (direction.x < -0.05f)
        {
            spriteTransform.localScale = sideRight;
        }
    }

}