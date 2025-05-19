using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    private float timer;
    private int spawnedCount = 0;
    public int maxEnemies = 10; // Set per spawner

    [Header("Path Settings")]
    public EnemyPath enemyPath;

    public int GetQuota() => maxEnemies;

    void Start()
    {
        // Optionally, register this spawner in a static list for quota summing
        EnemySpawnerRegistry.Register(this);
    }

    void Update()
    {
        if (spawnedCount >= maxEnemies)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedCount++;
            GameManager.Instance?.RegisterEnemy(enemy);

            // Assign the path to the enemy's path follower
            var pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null && enemyPath != null)
            {
                pathFollower.SetPath(enemyPath);
            }
        }
    }
}

// Helper registry for quota summing
public static class EnemySpawnerRegistry
{
    private static readonly System.Collections.Generic.List<EnemySpawner> spawners = new();

    public static void Register(EnemySpawner spawner)
    {
        if (!spawners.Contains(spawner))
            spawners.Add(spawner);
    }

    public static int GetTotalQuota()
    {
        int total = 0;
        foreach (var spawner in spawners)
            total += spawner.GetQuota();
        return total;
    }
}