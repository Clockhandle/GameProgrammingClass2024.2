using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_TrailAttackState : E1_MeleeAttackState
{
    private Enemy1TrailAttack enemy1;
    int attackCounter = 0;
    int trialAttackThresHold = 4;

    public E1_TrailAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, Enemy1TrailAttack enemy1) : base(entity, stateMachine, animBoolName, attackPosition, enemyData, enemy1)
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
        attackCounter++;

        if (attackCounter >= trialAttackThresHold)
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

    protected void TriggerTrailEffect()
    {
       

        if (entity.CurrentTar != null)
        {
          
            enemy1.SpawnTrailAttackEffect(); // Trail damage logic
        }
    }
}
