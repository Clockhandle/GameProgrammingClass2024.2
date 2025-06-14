using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class E3_FinalAttack : MeleeAttackState
{
    private Enemy3 enemy3;
    private int attackCount = 0;
    public GameObject pierceColliderPrefab;
    private FinalAttakPostProcessingEffect postProcessingEffect;
    public E3_FinalAttack(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, GameObject pierceColliderPrefab, FinalAttakPostProcessingEffect postProcessingEffect, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy3 = enemy3;   
        this.pierceColliderPrefab = pierceColliderPrefab;   
        this.postProcessingEffect = postProcessingEffect;
    }

    public override void Enter()
    {
        base.Enter();
        enemy3.anim.SetTrigger("FinalAttack");
    }

    public override void Exit()
    {
        base.Exit();
        enemy3.isInvincible = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinish)
        {
            stateMachine.ChangeState(enemy3.moveState);
           
        }
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        attackCount++;

        Vector2 size = Vector2.zero;

        if (attackCount == 1)
        {
            size = new Vector2(4.5f, 8f);
            Debug.Log("Box 1");
        }
        else if (attackCount == 2)
        {
            size = new Vector2(17.5f, 7f);
        }

        GameObject colliderObj = Object.Instantiate(pierceColliderPrefab, attackPosition.position, attackPosition.rotation);
        PierceAttackBoxCollider box = colliderObj.GetComponent<PierceAttackBoxCollider>();

        if (box != null)
        {
            box.Initialize(enemy3.enemyDataSO.damage + 15, LayerMask.GetMask("Unit"), size, (unitHit) =>
            {
                if (attackCount == 3)
                {
                   

                    postProcessingEffect?.ActivateEffect();

                    DealMassiveDamageToAllUnits();


                }
            });
        }
    }

    private void DealMassiveDamageToAllUnits()
    {
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
        foreach (var unit in allUnits)
        {
            if (unit.TryGetComponent(out Unit lmao))
            {
                lmao.TakeDamage(1000);
            }
        }
    }

    public override void TriggerAttackEnd()
    {
        base.TriggerAttackEnd();
    }
}
