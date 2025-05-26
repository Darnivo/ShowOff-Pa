using UnityEngine;

public class TurretShoot : MonoBehaviour
{

    public GameObject projectile;     // The projectile to fire
    public Transform firePoint;  
    public float shootInterval = 1f;    // Time between shots in seconds

    private float shootTimer = 0f;

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0f;
        }
    }

    void Shoot()
    {
        Instantiate(projectile, firePoint.position, firePoint.rotation);
        // Optional: add force to bullet if it has Rigidbody
        // Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // rb.AddForce(firePoint.forward * 10f, ForceMode.Impulse);
    }
}
