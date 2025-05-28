using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [Header("Particle Settings")]
    [SerializeField] ParticleSystem movementParticle;
    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;
    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [Header("References")]
    [SerializeField] Rigidbody rb;

    [Header("Ground-Check Settings")]
    [Tooltip("Empty child at player's feet")]
    [SerializeField] Transform groundCheck;
    [Tooltip("Radius of the ground-check sphere")]
    [SerializeField] float groundCheckRadius = 0.1f;
    [Tooltip("Which layers count as 'ground' (match your player)")]
    [SerializeField] LayerMask groundMask;

    [SerializeField] ParticleSystem landingParticle;
    private bool wasGrounded = false;


    float counter;

    private void Update()
    {
        counter += Time.deltaTime;

        bool isGrounded = Physics.CheckSphere(
        groundCheck.position,
        groundCheckRadius,
        groundMask,
        QueryTriggerInteraction.Ignore
    );

        if (isGrounded && !wasGrounded)
        {
            landingParticle.Play();
        }

        wasGrounded = isGrounded;

        if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > occurAfterVelocity)
        {
            if (counter > dustFormationPeriod)
            {
                movementParticle.Play();
                counter = 0;
            }
        }
        else
        {
            if (movementParticle.isPlaying)
                movementParticle.Stop();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
