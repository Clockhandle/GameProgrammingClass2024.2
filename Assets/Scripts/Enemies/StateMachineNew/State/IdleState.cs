using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    protected EnemyDataSO enemyDataSO;
    protected float idelTime;
    protected bool isFirstIdleOver;
    public IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO) : base(entity, stateMachine, animBoolName)
    {
        this.enemyDataSO = enemyDataSO;
    }

    public override void Enter()
    {
        base.Enter();
        // Stop moving
        entity.Move(false);

        isFirstIdleOver = false;
        SetIdleTime();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Time.time > startTime + idelTime)
        {
            isFirstIdleOver = true;
        }
    }
    private void SetIdleTime()
    {
        idelTime = enemyDataSO.idleTime;
    }
}
