using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float stopDistance = 3f;
    public float moveSpeed = 5f; // 속도 조절
    public int hp = 10;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed; // 시작할 때 적용
    }

    void Update()
    {
        if (target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            agent.SetDestination(target.position);
        }
        else
        {
            agent.ResetPath(); // 멈춤
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Destroy(gameObject);
        this.gameObject.SetActive(false);
    }
}
