using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_MeleeAttackState : MeleeAttackState
{
    private Enemy3 enemy3;
    private int attackCounter = 0;
    private int attackThreshold = 3;
    public E3_MeleeAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy3 = enemy3;   
    }

    public override void Enter()
    {
        base.Enter();
        isAnimationFinish = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        isUnitInMaxRange = enemy3.DetectUnit();

        if (isAnimationFinish)
        {
            if (isUnitInMaxRange)
            {

                stateMachine.ChangeState(enemy3.melleAttackState); // if enemy still detect move immediately to attack state
            }
            else if (enemy3.shouldEnterMovingImpale && !enemy3.hasPierceTriggered)
            {
                enemy3.hasPierceTriggered = true;
                enemy3.shouldEnterMovingImpale = false;
                stateMachine.ChangeState(enemy3.movingImpale);
            }
            else if (enemy3.shouldEnterFinalAttack && !enemy3.isFinalAttackActive)
            {
                enemy3.isFinalAttackActive = true;
                enemy3.shouldEnterFinalAttack = false;
                stateMachine.ChangeState(enemy3.finalAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy3.moveState);
            }

        }
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        if (enemy3.hasRevived)
        {
            attackCounter++;
            if (attackCounter >= attackThreshold)
            {
                attackCounter = 0;
                stateMachine.ChangeState(enemy3.circleAttackState);
            }
        }
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }
}
