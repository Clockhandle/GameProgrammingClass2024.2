using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_AoERangedAttackState : E2_RangedAttackState
{
    private Enemy2MultiShoot enemy2;

    public E2_AoERangedAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, Enemy2MultiShoot enemy2) : base(entity, stateMachine, animBoolName, attackPosition, enemyData, enemy2)
    {
        this.enemy2 = enemy2;
    }

    public override void TriggerAttack()
    {
        enemy2.ShootAoEProjectiles(); // Custom logic
    }


}
