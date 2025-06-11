using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Enemy Count")]
    public int totalEnemiesToDefeat = 10; // Set this from your spawners at game start
    private int enemiesDefeated = 0;

    [Header("UI")]
    public TMP_Text healthText;
    public TMP_Text enemyCountText;
    public GameObject gameOverPanel;
    public GameObject WinPanel;

    [Header("Spawners")]
    public List<EnemySpawner> spawners = new List<EnemySpawner>();

    private HashSet<GameObject> activeEnemies = new HashSet<GameObject>();
    private int enemySlainCounter = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;

        // Sum all quotas from spawners
        int total = 0;
        foreach (var spawner in spawners)
        {
            if (spawner != null)
                total += spawner.maxEnemies;
        }
        SetTotalEnemiesToDefeat(total);

        UpdateHealthUI();
        UpdateEnemyCountUI();

        LevelData currentLevelData = ProgressManager.Instance.selectedLevel;

        if (currentLevelData != null && currentLevelData.levelMusic != null)
        {
            SoundManager.Instance.ChangeMusic(currentLevelData.levelMusic);
        }

        WinPanel.SetActive(false);
        gameOverPanel.SetActive(false);

    }

    public void SetTotalEnemiesToDefeat(int total)
    {
        totalEnemiesToDefeat = total;
        UpdateEnemyCountUI();
    }

    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        IncrementEnemySlainCounter();
        UpdateEnemyCountUI();
        if (enemiesDefeated >= totalEnemiesToDefeat)
        {
            Debug.Log("Game Clear!");

            //string sceneToUnlock = ProgressManager.Instance.sceneOfNextLevelToUnlock;

            //// 2. If it's not empty, tell the ProgressManager to save it
            //if (!string.IsNullOrEmpty(sceneToUnlock))
            //{
            //    ProgressManager.Instance.UnlockLevel(sceneToUnlock);
            //}

            WinPanel.SetActive(true);
        }
    }

    public void OnEnemyReachedGoal()
    {
        currentHealth--;
        UpdateHealthUI();
        OnEnemyDefeated(); // Also count as defeated for the win condition
        if (currentHealth <= 0)
        {
            Debug.Log("Game Over!");
            gameOverPanel.SetActive(true);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"Health: {currentHealth}";
    }

    private void UpdateEnemyCountUI()
    {
        if (enemyCountText != null)
            enemyCountText.text = $"{enemiesDefeated}/{totalEnemiesToDefeat}";
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null)
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy != null)
            activeEnemies.Remove(enemy);
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    public void IncrementEnemySlainCounter()
    {
        enemySlainCounter++;
        // Optionally update UI or trigger events here
        Debug.Log($"Enemy slain! Total: {enemySlainCounter}");
    }

    public void ReturnToLevelSelector()
    {
        string sceneToUnlock = ProgressManager.Instance.sceneOfNextLevelToUnlock;

        // 2. If it's not empty, tell the ProgressManager to save it
        if (!string.IsNullOrEmpty(sceneToUnlock))
        {
            ProgressManager.Instance.UnlockLevel(sceneToUnlock);
        }
        ProgressManager.Instance.selectedCards.Clear();
        ProgressManager.Instance.selectedLevel = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelector");
    }
}
