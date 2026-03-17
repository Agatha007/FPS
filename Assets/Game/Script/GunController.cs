using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("ÃÑ ¼³Á¤")]
    public Transform[] guns;
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
    public GameObject shotEffect;

    public int maxBulletCount = 100;
    private int currentBulletCount = 0;

    [Header("ÃÑ¾Ë ÆÛÁü")]
    public float bulletSpread = 2f;

    [Header("ÃÑ¾Ë °ü¸®")]
    public Transform bulletRoot;
    public Transform hitRoot;

    [Header("ÃÑ »ç¿ë ¿©ºÎ")]
    public bool canShoot = true;

    public Transform playerCamera;

    [Header("Ç®¸µ")]
    public int bulletPoolSize = 100;
    public int hitPoolSize = 50;

    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> hitPool = new List<GameObject>();

    private float nextFireTime = 0f;

    private void Start()
    {
        SetGun();

        if (bulletRoot == null)
        {
            GameObject obj = new GameObject("Bullets");
            bulletRoot = obj.transform;
        }

        if (hitRoot == null)
        {
            GameObject obj = new GameObject("HitEffects");
            hitRoot = obj.transform;
        }
    }

    private void Update()
    {
        if (canShoot && Mouse.current.leftButton.isPressed)
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

        if (gun != null)
        {
            gun.localPosition = Vector3.Lerp(
                gun.localPosition,
                gunOriginalPos,
                gunReturnSpeed * Time.deltaTime);
        }


        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetGun(0);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetGun(1);
        }
    }

    private void SetGun(int index = 0)
    {
        foreach (var gun in guns)
            gun.gameObject.SetActive(false);

        gun = guns[index];
        gun.gameObject.SetActive(true);        

        var weapon = gun.GetComponent<Weapon>();
        gunKickbackZ = weapon.gunKickbackZ;
        gunReturnSpeed = weapon.gunReturnSpeed;
        bulletPrefab = weapon.bulletPrefab;
        firePoint = weapon.firePoint;
        bulletSpeed = weapon.bulletSpeed;
        fireRate = weapon.fireRate;
        bulletSpread = weapon.bulletSpread;
        gunOriginalPos = weapon.gunOriginalPos;
    }

    private void ShootProjectile()
    {
        if (currentBulletCount >= maxBulletCount) return;

        SoundManager.Instance.PlaySFX("shot");

        float spreadX = Random.Range(-bulletSpread, bulletSpread);
        float spreadY = Random.Range(-bulletSpread, bulletSpread);

        Vector3 shootDir =
            firePoint.forward +
            firePoint.right * spreadX * 0.01f +
            firePoint.up * spreadY * 0.01f;

        shootDir.Normalize();

        GameObject bullet = GetBullet();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.LookRotation(shootDir);
        bullet.SetActive(true);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(shootDir * bulletSpeed, ForceMode.VelocityChange);

        currentBulletCount++;

        ShotEffect();
        GunKick();
    }

    private void ShootRay()
    {
        SoundManager.Instance.PlaySFX("shot");

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
            if (hitEffect != null)
            {
                Vector3 pos = hit.point + hit.normal * 0.02f;

                GameObject effect = GetHitEffect();

                effect.transform.position = pos;
                effect.transform.rotation = Quaternion.LookRotation(hit.normal);
                effect.SetActive(true);

                StartCoroutine(DisableEffect(effect, 2f));
            }
        }

        ShotEffect();
        GunKick();
    }

    private GameObject GetBullet()
    {
        // ²¨Á®ÀÖ´Â ÃÑ¾Ë ¸ÕÀú Ã£±â
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }

        // ¾øÀ¸¸é »õ·Î »ý¼º
        GameObject obj = Instantiate(bulletPrefab, bulletRoot);
        obj.SetActive(false);

        bulletPool.Add(obj);

        return obj;
    }

    private GameObject GetHitEffect()
    {
        for (int i = 0; i < hitPool.Count; i++)
        {
            if (!hitPool[i].activeInHierarchy)
            {
                return hitPool[i];
            }
        }

        GameObject obj = Instantiate(hitEffect, hitRoot);
        obj.SetActive(false);

        hitPool.Add(obj);

        return obj;
    }

    private IEnumerator DisableEffect(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);

        if (obj != null)
            obj.SetActive(false);
    }

    private void GunKick()
    {
        if (gun == null) return;

        float zKick = -gunKickbackZ;

        gun.localPosition = gunOriginalPos + new Vector3(0f, 0f, zKick);
    }

    public void ShotEffect()
    {
        if (shotEffect == null)
            return;
        if (firePoint == null)
            return;

        GameObject obj = Instantiate(shotEffect, firePoint);
        obj.transform.localPosition = Vector3.zero;
    }

    public void RemoveBullet()
    {
        currentBulletCount--;

        if (currentBulletCount < 0)
            currentBulletCount = 0;
    }
}
