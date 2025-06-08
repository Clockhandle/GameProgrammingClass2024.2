using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_DeathState : DeadState
{
    private Enemy3 enemy3;
    public E3_DeathState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, enemyData)
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
    }
}
