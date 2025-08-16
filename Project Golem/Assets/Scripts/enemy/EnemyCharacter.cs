using UnityEngine;
using UnityEngine.AI;

public class EnemyCharacter : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("AI Settings")]
    public Transform target;
    public float detectionRange = 15f;
    public float attackRange = 2f;
    public float moveSpeed = 3.5f;

    private NavMeshAgent agent;

    private void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(target.position);

            if (distance <= attackRange)
            {
                Attack();
            }
        }
        if (!agent.isOnNavMesh) return;

        agent.SetDestination(target.position);

        // Simulate gravity if needed
        if (!agent.isOnOffMeshLink)
        {
            agent.baseOffset = Mathf.MoveTowards(agent.baseOffset, 0, Time.deltaTime * 1f);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Attack()
    {
        // Implement attack logic here (e.g., reduce player health)
        Debug.Log("Enemy attacks!");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        Destroy(gameObject);
    }
}
