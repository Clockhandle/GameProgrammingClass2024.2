using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_IdleState : IdleState
{
    private Enemy2 enemy2;
    public E2_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, enemyDataSO)
    {
        this.enemy2 = enemy2;   
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
        if (isFirstIdleOver)
        {
            stateMachine.ChangeState(enemy2.moveState);
        }
    }
}
