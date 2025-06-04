using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_DeadState : DeadState
{
    private Enemy1 enemy1;
    public E1_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy1 enemy1) : base(entity, stateMachine, animBoolName, enemyData)
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
    }
}
