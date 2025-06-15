using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_CircleAttackState : MeleeAttackState
{
    private Enemy3 enemy3;
    public GameObject aoePrefab;
    int circleAttackCount = 0;
    public E3_CircleAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, GameObject aoePrefab, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy3 = enemy3;
        this.aoePrefab = aoePrefab;
    }

   

    public override void Enter()
    {
        base.Enter();
        circleAttackCount++;
        enemy3.anim.SetTrigger("CircleAttackTrigger");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (isAnimationFinish)
        {
            if (circleAttackCount >= 3)
            {
                circleAttackCount = 0;
                stateMachine.ChangeState(enemy3.pierceAttackState);
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
        Debug.Log("CircleAttack - TriggerAttack()");

        if (aoePrefab != null)
        {
            GameObject aoe = Object.Instantiate(aoePrefab, attackPosition.position, Quaternion.identity);
            CircleAOEDamage aoeDamage = aoe.GetComponent<CircleAOEDamage>();

            if (aoeDamage != null)
            {
                aoeDamage.damage = enemy3.enemyDataSO.damage;
                aoeDamage.targetLayer = LayerMask.GetMask("Unit");
                aoeDamage.radius = 4;
            }
        }
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }
}