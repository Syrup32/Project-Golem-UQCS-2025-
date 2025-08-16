using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyAiTutorial enemyPrefab;          // your base enemy prefab (with EnemyAiTutorial + EnemyVisual + NavMeshAgent)
    public EnemySkinDefinition[] skins;          // drag all skin assets here
    public int count = 5;
    public float radius = 15f;

    public void SpawnWave()
    {
        if (enemyPrefab == null || skins == null || skins.Length == 0)
        {
            Debug.LogError("Spawner not configured.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 p = transform.position + Random.insideUnitSphere * radius;
            p.y = transform.position.y;

            var enemy = Instantiate(enemyPrefab, p, Quaternion.identity);
            var visual = enemy.GetComponent<EnemyVisual>();
            if (visual != null)
            {
                var skin = skins[Random.Range(0, skins.Length)];
                visual.ApplySkin(skin);
            }
        }
    }

    // optional: auto-spawn for testing
    private void Start() => SpawnWave();
}
