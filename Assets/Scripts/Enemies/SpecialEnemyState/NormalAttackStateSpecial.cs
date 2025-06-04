using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackStateSpecial : SpecialEnemyState
{

    protected bool hasAttacked = false;
    protected bool waitTotransis;
    public override void EnterState()
    {
        enemy.animator_refe.SetBool("isAttacking", true);
        hasAttacked = false;
        waitTotransis = false;

    }

    public override void UpdateState()
    {
        if ((enemy.CurrentTarget == null || enemy.CurrentTarget.isDead))
        {
            waitTotransis = true;

        }
        if(waitTotransis && hasAttacked)
        {
            stateMachine.ChangeState(enemy.GetComponent<WalkStateSpecial>());
            Debug.Log("Change State to walk affter attack");
        }

    }

    public void OnAttackAnimationEnd()
    {
        if (waitTotransis)
        {
            hasAttacked = true;
        }
      
    }



    public void OnAttackEvent()
    {
        enemy.DealDamageToUnit();
    }

    public override void ExitState()
    {
        enemy.animator_refe.SetBool("isAttacking", false);
    }
}
