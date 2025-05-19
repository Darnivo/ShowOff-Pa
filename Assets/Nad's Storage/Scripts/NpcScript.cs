using UnityEngine;

public class NpcScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    private float moveDurationMin = 1f;
    private float moveDurationMax = 3f;
    private float idleDurationMin = 1f;
    private float idleDurationMax = 2f;
    private float moveTimer = 0f;
    private float idleTimer = 0f;
    private int direction = 0; // 0: idle, -1: left, 1: right
    public bool isMoving = false;

    [Header("Chase Settings")]
    private bool isChasing;
    public Transform player;
    void Start()
    {
        StartMoving();
    }

    void Update()
    {
        if (isChasing)
        {
            startChasing();
        }
        else
        {
            ManageMovement();
        }
    }

    private void StartMoving()
    {
        isMoving = true;
        moveTimer = Random.Range(moveDurationMin, moveDurationMax);
        direction = Random.Range(0, 2) == 0 ? -1 : 1;
    }
    private void StartIdle()
    {
        isMoving = false;
        idleTimer = Random.Range(idleDurationMin, idleDurationMax);
        direction = 0;
    }

    private void ManageMovement()
    {
        if (isMoving)
        {
            moveTimer -= Time.deltaTime;
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
            if (moveTimer <= 0)
            {
                StartIdle();
            }
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                StartMoving();
            }
        }
    }
    public void SetToChase()
    {
        isChasing = true;
        isMoving = false;
        idleTimer = 0f;
        moveTimer = 0f;
        startChasing();
    }
    
    private void startChasing()
    {
        // Debug.Log("Chasing Player");
        transform.position = Vector3.MoveTowards(transform.position, player.position, Time.deltaTime * 2f);
    }
}
