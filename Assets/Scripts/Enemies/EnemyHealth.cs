using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyDataSO enemyData;
    private int currentHealth;

    void Awake()
    {
        if (enemyData != null)
            currentHealth = enemyData.maxHealth;
        else
            currentHealth = 1; // fallback
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
