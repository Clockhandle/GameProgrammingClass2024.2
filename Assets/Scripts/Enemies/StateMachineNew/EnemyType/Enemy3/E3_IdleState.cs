using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_IdleState : IdleState
{
    private Enemy3 enemy3;
    public E3_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, enemyDataSO)
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
        if (isFirstIdleOver)
        {
            stateMachine.ChangeState(enemy3.moveState);
        }
    }
}
