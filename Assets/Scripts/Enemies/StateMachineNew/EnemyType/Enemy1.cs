using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Entity
{
   public E1_IdleState idleState {  get; private set; } 
    public E1_MoveState moveState { get; private set; }
    //public E1_MelleAttackState melleAttackState { get; protected set; }
    public MeleeAttackState melleAttackState { get; protected set; }

    public E1_StuntState stuntState { get; private set; }   

    public E1_DeadState deadState { get; private set; }
    
    //public E1_TrailAttackState trailAttackState { get; protected set; }   

    [SerializeField] protected EnemyDataSO enemyData;

    [SerializeField] protected Transform attackPosition;

    public override void Start()
    {
        base.Start();
        this.enemyDataSO = enemyData;
        moveState = new E1_MoveState(this, stateMachine, "isWalking", enemyData, this);
        idleState = new E1_IdleState(this, stateMachine, "isIdle", enemyData, this);
        melleAttackState = new MeleeAttackState(this, stateMachine, "isAttacking", attackPosition,enemyData);
        stuntState = new E1_StuntState(this, stateMachine, "isStunned", enemyData, this);
        deadState = new E1_DeadState(this, stateMachine, "isDead", enemyData, this);
       

        stateMachine.Initialize(moveState);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }
}
