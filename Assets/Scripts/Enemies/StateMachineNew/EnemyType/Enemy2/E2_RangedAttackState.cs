using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_RangedAttackState : RangeAttackState
{
    private Enemy2 enemy2;
    private bool isWalkingAfterAttack;
    private float walkDuration = 2f;
 

    public E2_RangedAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy2 = enemy2;   
    }

    public override void Enter()
    {
        base.Enter();
        isWalkingAfterAttack = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinish)
        {
            stateMachine.ChangeState(enemy2.moveState); // Let MoveState handle post-walk
        }

    }
   


    public override void TriggerAttack()
    {
        base.TriggerAttack();
        enemy2.ShootProjectileAtTarget();
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
        if (isAnimationFinish)
        {
            enemy2.WasInAttackState = true;
            stateMachine.ChangeState(enemy2.moveState);
        }

    }
}
