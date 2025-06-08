using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_ReviveState : NewReviveState
{
    private Enemy3 enemy3;

    public E3_ReviveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO revivedData, RuntimeAnimatorController revivedAnimator, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, revivedData, revivedAnimator)
    {
        this.enemy3 = enemy3;   
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

    protected override void OnReviveComplete()
    {
        stateMachine.ChangeState(enemy3.moveState); // Or any buffed entry state
        if (enemy3 != null)
        {
            enemy3.normalCollider.enabled = false;
            enemy3.revivedCollider.enabled = true;
        }
    }
}
