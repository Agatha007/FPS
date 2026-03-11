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

    [Header("ÃÑ¾Ë ÆÛÁü")]
    public float bulletSpread = 2f;

    public Transform playerCamera;

    private float nextFireTime = 0f;

    private void Start()
    {
        if (gun != null)
            gunOriginalPos = gun.localPosition;
    }

    private void Update()
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

        // ÃÑ ¿øÀ§Ä¡ º¹±Í
        if (gun != null)
        {
            gun.localPosition = Vector3.Lerp(
                gun.localPosition,
                gunOriginalPos,
                gunReturnSpeed * Time.deltaTime
            );
        }
    }

    private void ShootProjectile()
    {
        if (currentBulletCount >= maxBulletCount) return;

        float spreadX = Random.Range(-bulletSpread, bulletSpread);
        float spreadY = Random.Range(-bulletSpread, bulletSpread);

        Vector3 shootDir =
            firePoint.forward +
            firePoint.right * spreadX * 0.01f +
            firePoint.up * spreadY * 0.01f;

        shootDir.Normalize();

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDir)
        );

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(shootDir * bulletSpeed, ForceMode.VelocityChange);

        currentBulletCount++;

        GunKick();
    }

    private void ShootRay()
    {
        float spreadX = Random.Range(-bulletSpread, bulletSpread);
        float spreadY = Random.Range(-bulletSpread, bulletSpread);

        Vector3 dir =
            playerCamera.forward +
            playerCamera.right * spreadX * 0.01f +
            playerCamera.up * spreadY * 0.01f;

        dir.Normalize();

        Ray ray = new Ray(playerCamera.position, dir);

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

    private void GunKick()
    {
        if (gun == null) return;

        float zKick = -gunKickbackZ;

        gun.localPosition = gunOriginalPos + new Vector3(0f, 0f, zKick);
    }

    public void RemoveBullet()
    {
        currentBulletCount--;

        if (currentBulletCount < 0)
            currentBulletCount = 0;
    }
}
