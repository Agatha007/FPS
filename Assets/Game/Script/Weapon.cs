using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("총")]
    public Transform gun;

    [Header("총 반동")]
    public float gunKickbackZ = 0.02f;
    public float gunReturnSpeed = 12f;

    [Header("총알/발사")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.1f;

    [Header("총알 퍼짐")]
    public float bulletSpread = 2f;

    [Header("리로드")]
    public int magazineSize = 30;     // 한 탄창
    public int currentAmmo;         // 현재 탄창 탄수
    public float reloadTime = 1f;     // 장전 시간

    [Header("리로드 회전 연출")]
    public Vector3 reloadRotation = new Vector3(0f, 0f, -25f); // Z축으로 기울기
    public float reloadRotateSpeed = 5f;
    public float reloadStayTime = 1f;

    [HideInInspector] public Vector3 gunOriginalPos;
    [HideInInspector] public Quaternion gunOriginalRot;

    private void Awake()
    {
        gunOriginalPos = transform.localPosition;
        gunOriginalRot = gun.localRotation;
        currentAmmo = magazineSize;
    }
}
