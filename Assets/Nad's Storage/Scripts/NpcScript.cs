using System;
using System.Collections;
using UnityEngine;

public class NpcScript : MonoBehaviour, IDeathHandler
{
    [Header("NPC Type")]
    public npcType thisNPC;
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float horizontalForce = 3f;
    public float jumpIntervalMin = 1f;
    public float jumpIntervalMax = 4f;
    public float jumpIntWhenChasing = 2f;
    public float closeRange = 3f;
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform frogGroundCheck;
    public float maxCheckDistance = 4f;


    private float jumpTimer;
    private Rigidbody rb;
    [Header("Respawn Settings")]
    public bool hasDesignatedRespawn = false;
    public Transform designatedRespawnPoint;
    public bool stopChasingAfterDeath = false;


    private bool isChasing;
    [Header("Chase Settings")]
    public Transform player;

    [Header("Steal Settings")]
    public float stealRange = 2f;
    public float stealDuration = 3f;
    private float stealTimer = 0f;
    private NpcManager npcManager;
    private Vector3 lastNPCPosition;
    public bool returnAfterStolen = false;
    private Vector3 initialPos;

    [Header("IF Sleeping NPC")]
    public float toAwakeDuration = 3f;
    public float toSleepDuration = 2f;

    private bool isAwake = false;
    private bool isInSight = false;
    private Coroutine awakeCoroutine;
    private Coroutine sleepCoroutine;
    private bool allowCamShake = false;
    private SpriteControl spriteControl;
    private FrogAnimation frog;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        ResetJumpTimer(jumpIntervalMax);
        npcManager = FindObjectOfType<NpcManager>();
        spriteControl = GetComponent<SpriteControl>();
        frog = GetComponent<FrogAnimation>();
        if (hasDesignatedRespawn)
        {
            lastNPCPosition = designatedRespawnPoint.position;
        }
        initialPos = transform.position;
    }

    void Update()
    {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f)
        {
            if (isChasing)
            {
                if (thisNPC == npcType.NORMAL_NPC || thisNPC == npcType.BIG_NPC)
                {
                    allowCamShake = true;
                    chaseJump();
                }
                else if (thisNPC == npcType.SLEEPING_NPC && isAwake == true)
                {
                    chaseJump();
                }
                ResetJumpTimer(jumpIntWhenChasing);
            }
            else
            {
                if (thisNPC == npcType.NORMAL_NPC)
                {
                    normalJump();
                }
                else if (thisNPC == npcType.SLEEPING_NPC && isAwake)
                {
                    normalJump();
                }
                ResetJumpTimer(jumpIntervalMax);
            }
        }
        gravity();
        if (frog != null) UpdateGroundDistance();
        if (isChasing)
        {
            checkPlayer();
        }
    }

    void FixedUpdate()
    {
        if (spriteControl != null)
        {
            spriteControl.flipNPC(rb.linearVelocity);
            if (frog != null) frog.frogJumps(rb.linearVelocity.y);
        }
    }

    private void normalJump()
    {
        int direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        Vector3 jumpForce = new Vector3(direction * horizontalForce, this.jumpForce, 0);
        rb.AddForce(jumpForce, ForceMode.Impulse);
        if (spriteControl != null) spriteControl.flipNPC(rb.linearVelocity);
        if (frog != null)
        {
            if (frog.isJumping == false) frog.setIsJumping(true);
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayFrogJumpSound();
            }
        }
    }
    private void chaseJump()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(player.position, transform.position);
        float force = distance < closeRange ? horizontalForce / 2f : horizontalForce;
        Vector3 jumpForce = new Vector3(direction.x * force, this.jumpForce, 0);
        rb.AddForce(jumpForce, ForceMode.Impulse);
        if (frog != null)
        {
            if (frog.isJumping == false) frog.setIsJumping(true);
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayFrogJumpSound();
            }
        }
    }
    public void SetToChase()
    {
        isChasing = true;
        if (thisNPC == npcType.BIG_NPC)
        {
            frog.frogAwake(true);
        }
    }
    public void DisableChase(bool isDeath = false)
    {
        isChasing = false;
        if (thisNPC == npcType.BIG_NPC)
        {
            frog.frogAwake(false);
        }
        if (returnAfterStolen == true && isDeath == false)
        {
            transform.position = initialPos;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private void ResetJumpTimer(float max)
    {
        jumpTimer = UnityEngine.Random.Range(jumpIntervalMin, max);
    }

    private void gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.down * 20f * Time.deltaTime;
        }
    }

    public void checkPlayer()
    {
        if (Vector3.Distance(player.position, transform.position) <= stealRange)
        {
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
        if (stopChasingAfterDeath)
        {
            DisableChase(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (frog != null) frog.setIsJumping(false);
        if (IsGroundLayer(collision.gameObject))
        {
            if (!hasDesignatedRespawn) lastNPCPosition = transform.position;
            if (thisNPC == npcType.BIG_NPC && allowCamShake)
            {
                npcManager.bignpc_jump();
                allowCamShake = false;
            }
        }
    }
    private bool IsGroundLayer(GameObject obj)
    {
        return (groundLayer.value & (1 << obj.layer)) != 0;
    }

    private void UpdateGroundDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(frogGroundCheck.position, Vector3.down, out hit, maxCheckDistance))
        {
            frog.setGroundDistance(hit.distance);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            if (thisNPC == npcType.SLEEPING_NPC)
            {
                isInSight = true;
                if (sleepCoroutine != null)
                {
                    StopCoroutine(sleepCoroutine);
                    sleepCoroutine = null;
                }
                if (awakeCoroutine == null)
                {
                    awakeCoroutine = StartCoroutine(wakeNPC());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            if (thisNPC == npcType.SLEEPING_NPC)
            {
                isInSight = false;
                if (awakeCoroutine != null)
                {
                    StopCoroutine(awakeCoroutine);
                    awakeCoroutine = null;
                }
                if (sleepCoroutine == null)
                {
                    sleepCoroutine = StartCoroutine(tosleepNPC());
                }
            }
        }
    }

    private IEnumerator wakeNPC()
    {
        yield return new WaitForSeconds(toAwakeDuration);
        if (isInSight) isAwake = true;
        awakeCoroutine = null;
    }

    private IEnumerator tosleepNPC()
    {
        yield return new WaitForSeconds(toSleepDuration);
        if (isInSight == false) isAwake = false;
        sleepCoroutine = null;
    }

    public enum npcType
    {
        NORMAL_NPC,
        BIG_NPC,
        SLEEPING_NPC
    }
}