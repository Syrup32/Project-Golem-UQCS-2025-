using UnityEngine;
using UnityEngine.AI;
using System.Collections;          // <-- needed for IEnumerator
// (optional if you use List<T> etc.)
using System.Collections.Generic;


public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    [Header("Projectile")]
    public GameObject projectile;
    public float projectileSpeed = 25f;
    
    //public Transform projectileSpawnPoint;
    private Transform projectileSpawnPoint;

    EnemyVisual _visual;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    public Renderer[] renderersToFlash;
    public AudioSource hitAudio;
    public AudioClip hitClip;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    
    //Legacy projectile system reinstate if needed
    /*public GameObject projectile;
    public Transform projectileContainer;
    public float projectileSpeed = 25f;
    */

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        _visual = GetComponent<EnemyVisual>();
        projectileSpawnPoint = _visual != null ? _visual.projectileSpawnPoint : null;
    }
    
    private void Start()
    {
        // If the spawner applied the skin after Awake, grab the socket now
        if (projectileSpawnPoint == null)
            projectileSpawnPoint = GetComponent<EnemyVisual>()?.projectileSpawnPoint;
    }

    private void Update()
    {
        //check for sight and attack
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange) Patroling();
        if(playerInSightRange && !playerInAttackRange) ChasePlayer();
        if(playerInAttackRange && playerInSightRange) AttackPlayer();
    }
    private void Patroling()
    {
        if(!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked && projectile != null)
        {
            if (projectileSpawnPoint == null) // safety
            {
                Debug.LogWarning($"{name}: projectileSpawnPoint is null; skipping attack this frame.");
                return;
            }

            Vector3 targetPos = new Vector3(player.position.x, projectileSpawnPoint.position.y, player.position.z);
            Vector3 direction = (targetPos - projectileSpawnPoint.position).normalized;

            Rigidbody rb = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.linearVelocity = direction * projectileSpeed;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (hitAudio && hitClip) hitAudio.PlayOneShot(hitClip);
        StartCoroutine(Flash());
        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    IEnumerator Flash()
    {
        const float t = 0.1f;
        foreach (var r in renderersToFlash) r.material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(t);
        foreach (var r in renderersToFlash) r.material.DisableKeyword("_EMISSION");
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
