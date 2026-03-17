using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("ÃÑ ¹Ýµ¿")]
    public float gunKickbackZ = 0.02f;
    public float gunReturnSpeed = 12f;

    [Header("ÃÑ¾Ë/¹ß»ç")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f;

    [Header("ÃÑ¾Ë ÆÛÁü")]
    public float bulletSpread = 2f;

    [HideInInspector] public Vector3 gunOriginalPos;

    private void Awake()
    {
        gunOriginalPos = transform.localPosition;
    }
}
