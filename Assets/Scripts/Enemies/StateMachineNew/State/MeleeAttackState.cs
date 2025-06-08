using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected EnemyDataSO enemyData;
   

    public MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData) : base(entity, stateMachine, animBoolName, attackPosition)
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        if (entity.CurrentTar != null)
        {
            entity.DealDamageToUnit();
        }
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
      
    }
}
