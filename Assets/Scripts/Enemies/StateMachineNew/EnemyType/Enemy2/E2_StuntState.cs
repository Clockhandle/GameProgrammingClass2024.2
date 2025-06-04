using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E2_StuntState : StuntState1
{
    private Enemy2 enemy2;
    public E2_StuntState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy2 enemy2) : base(entity, stateMachine, animBoolName, enemyData)
    {
        this.enemy2 = enemy2;   
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
                stateMachine.ChangeState(enemy2.rangedAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy2.moveState);
            }
        }
    }
}
