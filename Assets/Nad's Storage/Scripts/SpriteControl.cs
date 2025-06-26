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
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mouseWorldPos.x, transform.position.y, transform.position.z);

            if (mouseWorldPos.x > transform.position.x)
            {
                spriteTransform.localScale = sideLeft;
            }
            else if (mouseWorldPos.x < transform.position.x)
            {
                spriteTransform.localScale = sideRight;
            }

        }
        else
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput > 0)
            {
                spriteTransform.localScale = sideLeft;
            }
            else if (moveInput < 0)
            {
                spriteTransform.localScale = sideRight;
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