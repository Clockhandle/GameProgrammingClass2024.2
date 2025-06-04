using System.IO;
using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 2f;

    private float timer;
    private int spawnedCount = 0;
    public int maxEnemies = 10; // Set per spawner

    [Header("Path Settings")]
    public EnemyPath enemyPath;
    public int EnemyIndex;

    public int GetQuota() => maxEnemies;

    private void Awake()
    {
        
    }

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
            SpawnEnemy(EnemyIndex);
        }
    }

    void SpawnEnemy(int index)
    {
        if (index < 0 || index >= enemyPrefabs.Length)
        {
            Debug.LogWarning("Invalid enemy index");
            return;
        }

        GameObject enemyObj = Instantiate(enemyPrefabs[index], transform.position, Quaternion.identity);
        var pathFollower = enemyObj.GetComponent<EnemyPathFollower>();
        if (pathFollower != null)
        {
            pathFollower.SetPath(enemyPath);
        }

        GameManager.Instance?.RegisterEnemy(enemyObj);
        spawnedCount++;
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