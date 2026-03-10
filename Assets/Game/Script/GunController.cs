using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("ÃÑ ¼³Á¤")]
    public Transform gun;
    public float gunKickbackZ = 0.02f;
    public float gunReturnSpeed = 12f;

    private Vector3 gunOriginalPos;

    [Header("ÃÑ¾Ë/¹ß»ç")]
    public bool useRaycast = false;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f;
    public GameObject hitEffect;

    public int maxBulletCount = 100;
    private int currentBulletCount = 0;

    public Transform playerCamera;

    private float nextFireTime = 0f;

    void Start()
    {
        if (gun != null)
            gunOriginalPos = gun.localPosition;
    }

    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                if (useRaycast)
                    ShootRay();
                else
                    ShootProjectile();

                nextFireTime = Time.time + fireRate;
            }
        }

        // ÃÑ ¿øÀ§Ä¡ º¹±Í (¾ÕµÚ¸¸)
        if (gun != null)
        {
            gun.localPosition = Vector3.Lerp(gun.localPosition, gunOriginalPos, gunReturnSpeed * Time.deltaTime);
        }
    }

    void ShootProjectile()
    {
        if (currentBulletCount >= maxBulletCount) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);

        currentBulletCount++;
        GunKick();
    }

    void ShootRay()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.Log("Hit : " + hit.collider.name);

            if (hitEffect != null)
            {
                Vector3 pos = hit.point + hit.normal * 0.02f;

                Instantiate(
                    hitEffect,
                    pos,
                    Quaternion.LookRotation(hit.normal)
                );
            }
        }

        GunKick();
    }

    void GunKick()
    {
        if (gun == null) return;
        float zKick = -gunKickbackZ;
        gun.localPosition = gunOriginalPos + new Vector3(0f, 0f, zKick);
    }

    public void RemoveBullet()
    {
        currentBulletCount--;
        if (currentBulletCount < 0) currentBulletCount = 0;
    }
}
