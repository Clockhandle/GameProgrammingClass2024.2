using UnityEngine;

public class RangeeEnemy : EnemyBase
{
    public int debugCurrentHealth;

    // Override and do nothing — no trigger-based targeting
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Do nothing — ranged enemies use vision logic only
    }

    protected override void Update()
    {
        base.Update();
        debugCurrentHealth = currentHealth;
    }
}
