using UnityEngine;

public class SpriteControl : MonoBehaviour
{
    private Transform spriteTransform;
    public bool isSight = false;
    public float speed = 2f;
    private Camera mainCam;
    private Rigidbody rb;

    private Vector3 sideLeft;
    private Vector3 sideRight;


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
        checkDirection();

    }
    private void checkDirection() {
        if (isSight)
        {
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mouseWorldPos.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

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
            rb.linearVelocity = new Vector3(moveInput * speed, rb.linearVelocity.y, rb.linearVelocity.z);
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

}