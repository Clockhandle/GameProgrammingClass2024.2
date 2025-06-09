using System.Collections.Generic;
using UnityEngine;

public class E3_PierceAttackState : AttackState
{
    private Enemy3 enemy3;
    public GameObject pierceColliderPrefab;
    private int pierceHitCount = 0;
    private bool unitHitOnThirdStrike = false;

    public E3_PierceAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, GameObject pierceColliderPrefab, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.enemy3 = enemy3;
        this.pierceColliderPrefab = pierceColliderPrefab;
    }

    public override void Enter()
    {
        base.Enter();
        pierceHitCount = 0;
        unitHitOnThirdStrike = false;
        enemy3.anim.SetTrigger("PierceAttackTrigger");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinish)
        {
            if (enemy3.shouldEnterMovingImpale && !enemy3.hasPierceTriggered)
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
                // Normal state transition logic
                stateMachine.ChangeState(enemy3.moveState);
            }
        }
    }

    // Called from animation event (3x during anim)
    public void TriggerPierce()
    {
        pierceHitCount++;

        GameObject pierce = Object.Instantiate(pierceColliderPrefab, attackPosition.position, enemy3.transform.rotation);
        PierceCollider pierceComp = pierce.GetComponent<PierceCollider>();

        if (pierceComp != null)
        {
            pierceComp.Initialize(enemy3.enemyDataSO.damage, LayerMask.GetMask("Unit"), (unitHit) =>
            {
                if (pierceHitCount == 3 && unitHit)
                {
                    enemy3.Heal(30); 
                }
            });
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        TriggerPierce();
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }
}
