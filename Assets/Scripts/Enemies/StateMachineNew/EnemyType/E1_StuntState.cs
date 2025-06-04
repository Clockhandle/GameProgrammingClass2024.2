using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E1_StuntState : StuntState1
{
    private Enemy1 enemy1;
    public E1_StuntState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData, Enemy1 enemy1) : base(entity, stateMachine, animBoolName, enemyData)
    {
        this.enemy1 = enemy1;
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
            if(entity.CurrentTar!= null)
            {
                stateMachine.ChangeState(enemy1.melleAttackState);
            }
            else
            {
                stateMachine.ChangeState(enemy1.moveState);
            }
        }
    }
}
