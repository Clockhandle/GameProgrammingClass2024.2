using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAttackState : MeleeAttackState
{
    protected int attackCounter = 0;
    protected int trialAttackThresHold = 4;
    public TrailAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        attackCounter++;

        if(attackCounter >= trialAttackThresHold)
        {
            entity.DealDamageToUnit();
            TriggerTrailEffect();
            attackCounter = 0;
        }
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }

    protected virtual void TriggerTrailEffect()
    {
        Debug.Log("Trail effect triggered (override in subclass for actual logic).");
    }
}
