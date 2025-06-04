using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveState : State
{
    protected EnemyDataSO enemyData;
    public ReviveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData) : base(entity, stateMachine, animBoolName)
    {
        this.enemyData = enemyData; 
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
