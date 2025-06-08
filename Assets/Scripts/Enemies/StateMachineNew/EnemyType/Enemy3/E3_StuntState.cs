using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_StuntState : StuntState1
{
    Enemy3 enemy3;
    public E3_StuntState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, enemyData)
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
        if (isStuntOver)
        {
            if (entity.CurrentTar != null)
            {
                stateMachine.ChangeState(enemy3.melleAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy3.moveState);
            }
        }
    }
}
