using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    protected Transform attackPosition;
    protected bool isUnitInMaxRange;
    protected bool performedLongRange;

    protected bool isAnimationFinish;


    public AttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName)
    {
        this.attackPosition = attackPosition;   
    }

    public override void Enter()
    {
        base.Enter();
        isAnimationFinish = false;  
        entity.atsm.attackState = this; 

        entity.Move(false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public virtual void TriggerAttack()
    {

    }
    public virtual void TriggerAttackEnd()
    {
        isAnimationFinish=true; 
    }
}
