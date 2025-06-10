using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_MoveState : MoveState
{
    private Enemy3 enemy3;
    public E3_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, enemyDataSO)
    {
        this.enemy3 = enemy3;   
    }

    public override void Enter()
    {
        base.Enter();
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
            stateMachine.ChangeState(enemy3.idleState);
            return;
        }

        if (enemy3.shouldEnterFinalAttack && !enemy3.isFinalAttackActive)
        {
            enemy3.isFinalAttackActive = true;
            enemy3.shouldEnterFinalAttack = false;
            stateMachine.ChangeState(enemy3.finalAttackState);
        }


        if (isDetectUnit)
        {
            stateMachine.ChangeState(enemy3.melleAttackState);

            //TODO: transis to idle
            if (isDetectUnitBehind)
            {
                enemy3.Flip();
            }

            //  stateMachine.ChangeState(enemy1.idleState);
        }
    }
}
