using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GunController gunController;
    public GameObject hitEffect;

    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        Vector3 pos = contact.point - contact.normal * 0.1f;

        Instantiate(
            hitEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (gunController != null)
        {
            gunController.RemoveBullet();
        }
    }
}
