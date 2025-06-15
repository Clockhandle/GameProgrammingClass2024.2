using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// --- DATA STRUCTURES FOR THE NEW SYSTEM ---

/// <summary>
/// Defines a single group of identical enemies to be spawned sequentially.
/// </summary>
[System.Serializable]
public class EnemyGroup
{
    [Tooltip("The type of enemy to spawn in this group.")]
    public GameObject enemyPrefab;

    [Tooltip("The path this specific group of enemies will follow.")]
    public EnemyPathData pathForThisEnemy;

    [Tooltip("How many enemies to spawn in this group.")]
    public int count = 5;

    [Tooltip("The time delay between each spawn within this group.")]
    public float spawnInterval = 1.5f;
}

/// <summary>
/// Defines a single wave. A wave can contain multiple enemy groups
/// that will all start spawning at the same time (in parallel).
/// </summary>
[System.Serializable]
public class SpawnWave
{
    [Tooltip("Time in seconds to wait AFTER the previous wave is completely finished BEFORE this wave begins.")]
    public float delayBeforeWave = 10f;

    [Tooltip("A list of all enemy groups that will start spawning simultaneously in this wave.")]
    public List<EnemyGroup> enemyGroups;
}


// --- THE NEW ENEMY SPAWNER SCRIPT ---

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Configuration")]
    [Tooltip("A list of all waves that this spawner will execute in order.")]
    public List<SpawnWave> waves;

    void Start()
    {
        StartCoroutine(ProcessAllWaves());
    }

    /// <summary>
    /// Calculates the total number of enemies this spawner will create across all waves.
    /// Call this from your GameManager to set the win condition.
    /// </summary>
    /// <returns>The total count of all enemies to be spawned.</returns>
    public int GetTotalEnemyCount()
    {
        int totalCount = 0;
        foreach (var wave in waves)
        {
            foreach (var group in wave.enemyGroups)
            {
                totalCount += group.count;
            }
        }
        return totalCount;
    }

    /// <summary>
    /// The main coroutine that iterates through each wave sequentially.
    /// </summary>
    private IEnumerator ProcessAllWaves()
    {
        foreach (var wave in waves)
        {
            // 1. Wait for the specified delay before this wave starts.
            yield return new WaitForSeconds(wave.delayBeforeWave);

            // --- THIS IS THE NEW LOGIC TO WAIT FOR THE WAVE TO FINISH ---
            // 2. Create a list to keep track of the running spawn coroutines for this wave.
            List<Coroutine> runningGroupSpawners = new List<Coroutine>();

            // 3. Start a new coroutine for each enemy group in the wave and add it to our tracking list.
            foreach (var group in wave.enemyGroups)
            {
                Coroutine groupCoroutine = StartCoroutine(SpawnEnemyGroup(group));
                runningGroupSpawners.Add(groupCoroutine);
            }

            // 4. Now, wait for every single coroutine in our tracking list to finish.
            //    The main coroutine will pause here until the last enemy of the current wave has been spawned.
            foreach (var coroutine in runningGroupSpawners)
            {
                yield return coroutine;
            }
            // --- END NEW LOGIC ---

            Debug.Log("A wave has finished spawning all its enemies.");
        }

        Debug.Log("Spawner has finished all waves.");
    }

    /// <summary>
    /// A coroutine responsible for spawning a single group of sequential enemies.
    /// </summary>
    private IEnumerator SpawnEnemyGroup(EnemyGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            SpawnSingleEnemy(group);
            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    /// <summary>
    /// Instantiates one enemy and assigns it the correct path.
    /// </summary>
    private void SpawnSingleEnemy(EnemyGroup group)
    {
        if (group.enemyPrefab == null || group.pathForThisEnemy == null)
        {
            Debug.LogWarning("An enemy group is missing a prefab or a path!", this);
            return;
        }

        GameObject enemyObj = Instantiate(group.enemyPrefab, transform.position, Quaternion.identity);

        var pathFollower = enemyObj.GetComponent<EnemyPathFollower>();
        var directionHandler = enemyObj.GetComponent<EnemyDirectionHandler>();

        if (pathFollower != null && directionHandler != null)
        {
            var runtimePath = enemyObj.AddComponent<EnemyPath>();
            runtimePath.Initialize(group.pathForThisEnemy.checkpoints, directionHandler);
            pathFollower.SetPath(runtimePath);
        }

        GameManager.Instance?.RegisterEnemy(enemyObj);
    }
}