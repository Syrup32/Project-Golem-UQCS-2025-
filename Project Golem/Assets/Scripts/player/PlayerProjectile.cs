using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    public float damage = 25f;

    private void Start()
    {
        Destroy(gameObject, 5f); // Failsafe to clean up if it never hits anything
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            EnemyAiTutorial enemy = other.GetComponentInParent<EnemyAiTutorial>(); // 👈 Important change
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
                Debug.Log($"Hit enemy: {enemy.name}, dealt {damage} damage");
            }
            else
            {
                Debug.LogWarning($"Hit object on enemy layer but no EnemyAiTutorial script found: {other.name}");
            }

            Destroy(gameObject);
        }

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Debug.Log("hit ground!");
            Destroy(this.gameObject);
        }
    }
}
