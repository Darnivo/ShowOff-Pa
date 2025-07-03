using System.Collections.Generic;
using UnityEngine;

public class TurretShoot : MonoBehaviour
{

    // public GameObject projectile;     // The projectile to fire
    public List<GameObject> projectileList = new List<GameObject>(); 
    public Transform firePoint;  
    public float shootInterval = 1f;    // Time between shots in seconds

    private float shootTimer = 0f;

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            if (projectileList.Count < 0)
            {
                Debug.LogError("ProjectileList empty"); 
            }
            GameObject randomProjectile = projectileList[Random.Range(0, projectileList.Count)]; 
            Shoot(randomProjectile);
            shootTimer = 0f;
        }
    }

    void Shoot(GameObject projectile)
    {
        Instantiate(projectile, firePoint.position, firePoint.rotation);
        // Optional: add force to bullet if it has Rigidbody
        // Rigidbody rb = bullet.GetComponent<Rigidbody>();
        // rb.AddForce(firePoint.forward * 10f, ForceMode.Impulse);
    }
}
