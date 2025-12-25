using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 40f;
    public float fireRate = 0.15f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.timeScale == 0f) return;   // prevent firing when paused

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }


    void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        Vector3 shootDirection;

        if (Physics.Raycast(ray, out RaycastHit hit))
            shootDirection = (hit.point - firePoint.position).normalized;
        else
            shootDirection = ray.direction;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(shootDirection * fireForce, ForceMode.VelocityChange);
    }
}
