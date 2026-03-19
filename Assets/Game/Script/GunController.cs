using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    [Header("총 설정")]
    public Transform[] guns;
    public Transform gun;
    public float gunKickbackZ = 0.02f;
    public float gunReturnSpeed = 12f;

    private Vector3 gunOriginalPos;
    private Quaternion gunOriginalRot;

    [Header("총알/발사")]    
    public bool useRaycast = false;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f;
    public GameObject hitEffect;
    public GameObject shotEffect;

    public int maxBulletCount = 100;
    private int currentBulletCount = 0;

    [Header("총알 퍼짐")]
    public float bulletSpread = 2f;

    [Header("총알 관리")]
    public Transform bulletRoot;
    public Transform hitRoot;

    [Header("총 사용 여부")]
    public bool canShoot = true;

    public Transform playerCamera;

    [Header("풀링")]
    public int bulletPoolSize = 100;
    public int hitPoolSize = 50;

    private List<GameObject> bulletPool = new List<GameObject>();
    private List<GameObject> hitPool = new List<GameObject>();

    private float nextFireTime = 0f;

    [Header("리로드")]
    public int magazineSize = 30;     // 한 탄창
    public int currentAmmo;           // 현재 탄창 탄수
    public float reloadTime = 2f;     // 장전 시간
    private bool isReloading = false;

    [Header("리로드 회전 연출")]
    public Vector3 reloadRotation = new Vector3(0f, 0f, -25f); // Z축으로 기울기
    public float reloadRotateSpeed = 5f;
    public float reloadStayTime = 1f;

    private Weapon weapon;
    private Coroutine reloadCoroutine;

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
        if (canShoot && !isReloading && Mouse.current.leftButton.isPressed)
        {
            if (Time.time >= nextFireTime)
            {
                if (currentAmmo > 0)
                {
                    if (useRaycast)
                        ShootRay();
                    else
                        ShootProjectile();

                    currentAmmo--;
                    nextFireTime = Time.time + fireRate;

                    if(currentAmmo <= 0)
                        reloadCoroutine = StartCoroutine(Reload());
                }
            }
        }

        if (gun != null && !isReloading)
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

        // 리로드 (R키)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!isReloading && currentAmmo < magazineSize)
            {
                reloadCoroutine = StartCoroutine(Reload());
            }
        }
    }

    private void SetGun(int index = 0)
    {
        // 리로드 중이면 강제 종료
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
            isReloading = false;
        }

        foreach (var gun in guns)
            gun.gameObject.SetActive(false);

        gun = guns[index];
        gun.gameObject.SetActive(true);

        weapon = gun.GetComponent<Weapon>();
        gunKickbackZ = weapon.gunKickbackZ;
        gunReturnSpeed = weapon.gunReturnSpeed;
        bulletPrefab = weapon.bulletPrefab;
        firePoint = weapon.firePoint;
        bulletSpeed = weapon.bulletSpeed;
        fireRate = weapon.fireRate;
        bulletSpread = weapon.bulletSpread;
        gunOriginalPos = weapon.gunOriginalPos;
        gunOriginalRot = weapon.gunOriginalRot;
        magazineSize = weapon.magazineSize;
        currentAmmo = weapon.magazineSize;
        reloadRotation = weapon.reloadRotation;

        weapon.gun.localRotation = gunOriginalRot;
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
        // 꺼져있는 총알 먼저 찾기
        for (int i = 0; i < bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeInHierarchy)
            {
                return bulletPool[i];
            }
        }

        // 없으면 새로 생성
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

    private IEnumerator Reload()
    {
        isReloading = true;

        var gun = weapon.gun;
        Quaternion startRot = gun.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(reloadRotation);

        float t = 0;

        // 1. 기울이기
        while (t < 1f)
        {
            t += Time.deltaTime * reloadRotateSpeed;
            gun.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        SoundManager.Instance.PlaySFX("reload");

        // 2. 잠깐 멈춤
        yield return new WaitForSeconds(reloadStayTime);

        // 3. 원위치 복귀
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * reloadRotateSpeed;
            gun.localRotation = Quaternion.Slerp(targetRot, startRot, t);
            yield return null;
        }

        gun.localRotation = startRot;

        currentAmmo = magazineSize;
        isReloading = false;
    }
}
