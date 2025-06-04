using UnityEngine;

public class RangedEnemyPathFollower : EnemyPathFollower
{
    private EnemyRanged enemyRanged;

    protected override void Awake()
    {
        base.Awake();
        enemyRanged = GetComponent<EnemyRanged>();
    }

    protected override void Update()
    {
        // If currently attacking, stop moving
        if (enemyRanged != null && enemyRanged.isAttacking)
        {
            return;
        }

        // If movement is allowed, follow the path
        if (enemyRanged != null && enemyRanged.isWalking)
        {
            base.Update();
        }
    }
}
