using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public GunController gunController;
    [HideInInspector] public float lifeTime = 3f;

    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
            StopCoroutine(disableCoroutine);

        disableCoroutine = StartCoroutine(DisableAfterTime(lifeTime));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
            return;

        ContactPoint contact = collision.contacts[0];

        GameObject effect = gunController.GetHitEffect();
        effect.transform.position = contact.point + contact.normal * 0.15f;
        effect.transform.rotation = Quaternion.LookRotation(contact.normal);
        effect.SetActive(true);

        gunController.DisableEffect(effect, 2f);
        gunController.RemoveBullet();

        gameObject.SetActive(false);        
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);

        gunController.RemoveBullet();
    }
}
