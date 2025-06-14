using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAttackState : MeleeAttackState
{
   
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
    
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }

   
}
