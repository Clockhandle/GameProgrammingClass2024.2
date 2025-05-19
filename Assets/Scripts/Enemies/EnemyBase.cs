using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyDataSO enemyData;

    protected int currentHealth;
    protected bool isDead = false;

    protected virtual void Awake()
    {
        if (enemyData == null)
            Debug.LogError("EnemyDataSO not assigned!", this);

        currentHealth = enemyData != null ? enemyData.maxHealth : 1;
    }

    protected virtual void Start()
    {
        OnSpawn();
    }

    protected virtual void Update()
    {
        OnUpdate();
    }

    protected virtual void OnDestroy()
    {
        OnDeath();
    }

    // --- Common Logic ---

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // Unified: Notify GameManager that this enemy is defeated (slain or reached goal)
        GameManager.Instance?.OnEnemyDefeated();

        // Add death effects, drop loot, etc.
        Destroy(gameObject);
    }

    // --- Extensible Hooks ---

    protected virtual void OnSpawn() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDeath() { }
}
