using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntState1 : State
{
    protected EnemyDataSO enemyData;
    protected bool isStuntOver;

    public StuntState1(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData) : base(entity, stateMachine, animBoolName)
    {
        this.enemyData = enemyData; 
    }

    public override void Enter()
    {
        base.Enter();
        isStuntOver = false;
        entity.Move(false);
        Debug.Log("Stunt Begin");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Time.time >= startTime + enemyData.stuntTime)
        {
            isStuntOver = true;
            entity.Move(true);
            Debug.Log("Stunt end");
        }
    }
}
