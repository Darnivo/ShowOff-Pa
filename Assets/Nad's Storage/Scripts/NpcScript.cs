using Mono.Cecil;
using UnityEngine;

public class NpcScript : MonoBehaviour, IDeathHandler
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float horizontalForce = 3f;
    public float jumpIntervalMin = 1f;
    public float jumpIntervalMax = 4f;
    public float jumpIntWhenChasing = 2f;
    public float closeRange = 3f;

    private float jumpTimer;
    private Rigidbody rb;
    private bool isGrounded = true;
    [Header("Ground Check")]
    public LayerMask groundLayer;
    private float groundCheckDistance = 0.1f;


    [Header("Chase Settings")]
    private bool isChasing;
    public Transform player;

    [Header("Steal Settings")]
    public float stealRange = 2f;
    public float stealDuration = 3f;
    private float stealTimer = 0f;
    private NpcManager npcManager;
    private Vector3 lastNPCPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        ResetJumpTimer(jumpIntervalMax);
        npcManager = FindObjectOfType<NpcManager>();
    }

    void Update()
    {
        // isGrounded = IsGrounded();
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f)
        {
            if (isChasing)
            {
                chaseJump();
                ResetJumpTimer(jumpIntWhenChasing);
            }
            else
            {
                normalJump();
                ResetJumpTimer(jumpIntervalMax);
            }

        }
        gravity();
        if (isChasing)
        {
            checkPlayer();
        }

    }

    private void normalJump()
    {
        int direction = Random.Range(0, 2) == 0 ? -1 : 1;
        Vector3 jumpForce = new Vector3(direction * horizontalForce, this.jumpForce, 0);
        rb.AddForce(jumpForce, ForceMode.Impulse);
    }
    private void chaseJump()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);
        float force = distance < closeRange ? horizontalForce / 2f : horizontalForce;
        Vector3 jumpForce = new Vector3(direction.x * force, this.jumpForce, 0);
        rb.AddForce(jumpForce, ForceMode.Impulse);
    }
    public void SetToChase()
    {
        isChasing = true;
    }
    public void DisableChase()
    {
        isChasing = false;
    }
    // bool IsGrounded()
    // {
    //     return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

    // }

    private void ResetJumpTimer(float max)
    {
        jumpTimer = Random.Range(jumpIntervalMin, max);
    }

    private void gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.down * 20f * Time.deltaTime; // simulate stronger gravity only when falling
        }

    }

    public void checkPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) <= stealRange)
        {
            // Debug.Log("Player within stealing range"); 
            stealTimer += Time.deltaTime;
            if (stealTimer >= stealDuration)
            {
                Debug.Log("Stealing key");
                npcManager.NPC_DisableChasePlayer();
                stealTimer = 0f;
            }
        }
        else
        {
            stealTimer = 0f;
        }

    }

    public void onDeath()
    {
        transform.position = new Vector3(lastNPCPosition.x, lastNPCPosition.y + 5f, lastNPCPosition.z);
        rb.linearVelocity = Vector3.zero;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            lastNPCPosition = transform.position;
        }
    }
    private bool IsGroundLayer(GameObject obj) {
        return (groundLayer.value & (1 << obj.layer)) != 0;
    }


}
