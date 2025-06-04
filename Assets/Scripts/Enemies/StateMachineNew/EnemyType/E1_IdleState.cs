using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class E1_IdleState : IdleState
{
    private Enemy1 enemy1;
    public E1_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO, Enemy1 enemy1) : base(entity, stateMachine, animBoolName, enemyDataSO)
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
        if (isFirstIdleOver)
        {
            stateMachine.ChangeState(enemy1.moveState);
        }
    }
}
