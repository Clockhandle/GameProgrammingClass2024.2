using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_TrailAttackState : TrailAttackState
{
    private Enemy1TrailAttack enemy1;
    public E1_TrailAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, Enemy1TrailAttack enemy1) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
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

        isUnitInMaxRange = enemy1.DetectUnit();

        if (isAnimationFinish)
        {
            if (isUnitInMaxRange)
            {

                stateMachine.ChangeState(enemy1.moveState); // if enemy still detect move immediately to attack state
            }
            else
            {
                stateMachine.ChangeState(enemy1.moveState);
            }

        }
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }

    protected override void TriggerTrailEffect()
    {
        base.TriggerTrailEffect();

        if (entity.CurrentTar != null)
        {
          
            enemy1.SpawnTrailAttackEffect(); // Trail damage logic
        }
    }
}
