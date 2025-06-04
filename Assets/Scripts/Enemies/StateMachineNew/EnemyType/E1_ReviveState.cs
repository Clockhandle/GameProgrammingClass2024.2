using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_ReviveState : ReviveState
{
    private Enemy1Revive enemy1;
    private float reviveTime = 1;
   

    public E1_ReviveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy1Revive enemy1) : base(entity, stateMachine, animBoolName, enemyData)
    {
        this.enemy1 = enemy1;
    }

    public override void Enter()
    {
        base.Enter();
        enemy1.SetReviveEffects(); // switch to revive animations
                                   // enemy1.anim.SetTrigger("Revive");
      
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= startTime + reviveTime)
        {
            enemy1.ResetHealth();

            stateMachine.ChangeState(enemy1.moveState);
        }
    }
}
