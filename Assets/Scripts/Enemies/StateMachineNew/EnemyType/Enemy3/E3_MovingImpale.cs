using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_MovingImpale : MeleeAttackState
{
    private Enemy3 enemy3;
    private int attackCount = 0;
    public GameObject pierceColliderPrefab;
    public E3_MovingImpale(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, GameObject pierceColliderPrefab, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy3 = enemy3;
        this.pierceColliderPrefab = pierceColliderPrefab;
    }

    public override void Enter()
    {
        base.Enter();


        enemy3.anim.SetTrigger("MovingImpale");
  
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (isAnimationFinish)
        {
            if (enemy3.shouldEnterFinalAttack && !enemy3.isFinalAttackActive)
            {
                enemy3.isFinalAttackActive = true;
                enemy3.shouldEnterFinalAttack = false;
                stateMachine.ChangeState(enemy3.finalAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy3.melleAttackState);
            }
           
        }
    }


    public override void TriggerAttack()
    {
        base.TriggerAttack();
        attackCount++;

        Vector2 size = Vector2.zero;



        if (attackCount == 1)
        {
            size = new Vector2(5.5f, 8f);
            Debug.Log("Box 1");
        }
        else if (attackCount == 2)
        {
            size = new Vector2(28f, 6f);
        }

        GameObject colliderObj = Object.Instantiate(pierceColliderPrefab, attackPosition.position, attackPosition.rotation);
        PierceAttackBoxCollider box = colliderObj.GetComponent<PierceAttackBoxCollider>();

        if (box != null)
        {
            box.Initialize(enemy3.enemyDataSO.damage + 15, LayerMask.GetMask("Unit"), size, (unitHit) =>
            {
                if (attackCount == 3)
                {
                    enemy3.anim.speed = 0f;
                    enemy3.StartCoroutine(HealSmallAmountOverTime());
                }
            });
        }

    }

    private IEnumerator HealSmallAmountOverTime()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(2);
            enemy3.Heal(10);  // heal 10 health every 2 econd for 10 times
        }
        enemy3.anim.speed = 1f;
    }

    public override void Exit()
    {
        base.Exit();
        attackCount = 0;
    }

  
}