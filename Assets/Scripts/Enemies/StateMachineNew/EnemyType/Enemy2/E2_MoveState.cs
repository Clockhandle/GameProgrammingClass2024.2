using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_MoveState : MoveState
{
    private Enemy2 enemy2;
    protected bool isEnemyInRange;
    private bool isWalkingAfterAttack;
    public E2_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, enemyDataSO)
    {
        this.enemy2 = enemy2;   
    }

    public override void Enter()
    {
        base.Enter();
        isEnemyInRange = entity.DetectLongRange();

        if (enemy2.WasInAttackState)
        {
            isWalkingAfterAttack = true;
            startTime = Time.time;
            entity.Move(true); // Start walking
            enemy2.WasInAttackState = false; // Reset flag
        }
        else
        {
            isWalkingAfterAttack = false;
            entity.Move(true); // Normal move
        }

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (!hasDoneFirstIdle && isFirstTimeMoving == false)
        {
            hasDoneFirstIdle = true;
            stateMachine.ChangeState(enemy2.idleState);
            return;
        }

        if (isWalkingAfterAttack)
        {
            if (Time.time - startTime >= enemyDataSO.walkDurrationRanged)
            {
                isWalkingAfterAttack = false;

                if (entity.DetectLongRange())
                {
                    stateMachine.ChangeState(enemy2.rangedAttackState);
                    return;
                }
            }
        }
        else
        {
            isEnemyInRange = entity.DetectLongRange();

            if (isEnemyInRange)
            {
                stateMachine.ChangeState(enemy2.rangedAttackState);
            }
        }
    }
}

