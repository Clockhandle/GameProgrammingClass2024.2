using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    protected EnemyDataSO enemyDataSO;
    protected bool isDetectUnit;
    protected bool isDetectUnitBehind;
    protected bool hasFlip;

    //For firt moving 
    protected float firstMoveDuration = 1f;
    protected bool isFirstTimeMoving = true;
    protected bool hasDoneFirstIdle = false;



    public MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyDataSO) : base(entity, stateMachine, animBoolName)
    {
        this.enemyDataSO = enemyDataSO;
    }

    public override void Enter()
    {
        base.Enter();
        entity.Move(true);
        isDetectUnit = entity.DetectUnit();
      //  isDetectUnitBehind = entity.DetectUnitBehind();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

       // entity.Move(true);

        if (isFirstTimeMoving && Time.time >= startTime + firstMoveDuration)
        {
            isFirstTimeMoving = false;
         
        }

        isDetectUnit = entity.DetectUnit();
       // isDetectUnitBehind = entity.DetectUnitBehind();
    }


    public override void Exit()
    {
        base.Exit();
       
    }

   
    

}
