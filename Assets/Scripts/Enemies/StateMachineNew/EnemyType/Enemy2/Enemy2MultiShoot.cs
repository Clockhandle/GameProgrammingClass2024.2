using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2MultiShoot : Enemy2
{
    public override void Start()
    {
        base.Start();

        // Override the ranged attack state to use AoE version
        rangedAttackState = new E2_AoERangedAttackState(this, stateMachine, "isAttacking", firePoint, enemyData, this);
        stateMachine.Initialize(moveState); // Restart with correct state
    }

    public void ShootAoEProjectiles()
    {
        float range = enemyData.longRange;
        Vector2 center = firePoint.position;

        for (int i = 0; i < 5; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * range;
            Vector2 targetPosition = center + randomOffset;

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            AoEProjectile aoe = proj.GetComponent<AoEProjectile>();
            if (aoe != null)
            {
                aoe.Initialize(transform.position, targetPosition, enemyDataSO.damage);
            }
        }
    }

}
