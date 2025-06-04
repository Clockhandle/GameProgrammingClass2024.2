using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_MoveState : MoveState
{
    private Enemy1 enemy1;
    public E1_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy1 enemy1) : base(entity, stateMachine, animBoolName, enemyDataSO)
    {
        this.enemy1 = enemy1;
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
            stateMachine.ChangeState(enemy1.idleState);
            return;
        }



        if (isDetectUnit)
        {
            stateMachine.ChangeState(enemy1.melleAttackState);

            //TODO: transis to idle
            if (isDetectUnitBehind)
            {
                enemy1.Flip();
            }

          //  stateMachine.ChangeState(enemy1.idleState);
        }
    }
}
