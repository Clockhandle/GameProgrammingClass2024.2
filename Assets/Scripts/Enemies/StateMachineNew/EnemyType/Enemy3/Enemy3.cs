using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : Entity
{

    private bool hasPierceTriggered = false;

    public bool hasRevived = false;
    public E3_IdleState idleState { get; private set; }
    public E3_MoveState moveState { get; private set; }
    //public E1_MelleAttackState melleAttackState { get; protected set; }
    public E3_MeleeAttackState melleAttackState { get; protected set; }

    public E3_StuntState stuntState { get; private set; }

    public E3_DeathState deadState { get; private set; }

    public E3_CircleAttackState circleAttackState { get; protected set; }

    public E3_ReviveState reviveState { get; protected set; }

      public E3_PierceAttackState pierceAttackState { get; protected set; }   

    //   public E3_MovingImpale movingImpale { get; protected set; }



    [SerializeField] protected EnemyDataSO enemyData;

    [SerializeField] protected Transform attackPosition;
    [SerializeField] protected Transform priecePosition;

    [SerializeField] protected float aoeCircleRadius;

    // ReviveState Hanlder
    [Header("Revive Settings")]
    public EnemyDataSO revivedDataSO;
    public RuntimeAnimatorController revivedAnimator;


    public Collider2D normalCollider;
    public Collider2D revivedCollider;
    public GameObject aoePrefab;
    public GameObject piercePrefab;

    public override void Start()
    {
        base.Start();
        this.enemyDataSO = enemyData;
        moveState = new E3_MoveState(this, stateMachine, "isWalking", enemyData, this);
        idleState = new E3_IdleState(this, stateMachine, "isIdle", enemyData, this);
        melleAttackState = new E3_MeleeAttackState(this, stateMachine, "isAttacking", attackPosition, enemyData, this);
        stuntState = new E3_StuntState(this, stateMachine, "isStunned", enemyData, this);
        deadState = new E3_DeathState(this, stateMachine, "isDead", enemyData, this);
        reviveState = new E3_ReviveState(this, stateMachine, "isRevive", revivedDataSO, revivedAnimator, this);
        circleAttackState = new E3_CircleAttackState(this, stateMachine, "isCircleAttacking", attackPosition, enemyDataSO, aoePrefab, this);
        pierceAttackState = new E3_PierceAttackState(this, stateMachine, "isPiercing", priecePosition, piercePrefab, this);


        stateMachine.Initialize(moveState);
    }

    protected override void Die()
    {
        if (hasRevived)
        {
            base.Die();
            stateMachine.ChangeState(new DeadState(this, stateMachine, "isDead", enemyDataSO));
        }
        else
        {
            hasRevived = true;
            stateMachine.ChangeState(reviveState);
            enemyData = revivedDataSO;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, enemyDataSO.maxHealth);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public override void Update()
    {
        if (!hasPierceTriggered && currentHealth <= enemyDataSO.maxHealth * 0.3f)
        {
            if (DetectTargetInPierceRange())
            {
                hasPierceTriggered = true;
               // stateMachine.ChangeState(movingPierceState);
            }
        }
    }

    bool DetectTargetInPierceRange()
    {
        Vector2 boxSize = new Vector2(10f, 2f); // wide straight box
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, transform.right, 0, LayerMask.GetMask("Unit"));
        return hit.collider != null;
    }

}
