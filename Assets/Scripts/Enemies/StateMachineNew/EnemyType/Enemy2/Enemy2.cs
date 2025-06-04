using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Entity
{
    public bool WasInAttackState { get; set; }
    public E2_MoveState moveState {  get; private set; }
    public E2_IdleState idleState { get; private set; }

    public E2_RangedAttackState rangedAttackState { get; protected set; } 

    public E2_DeadState deadState { get; private set; } 
    public E2_StuntState stuntState { get; private set; }

    [SerializeField] protected EnemyDataSO enemyData;
    public Transform firePoint;
    public GameObject projectilePrefab;

    public override void Start()
    {
        base.Start();
        moveState = new E2_MoveState(this, stateMachine, "isWalking", enemyData, this);
        idleState = new E2_IdleState(this, stateMachine, "isIdle", enemyData, this);
        rangedAttackState = new E2_RangedAttackState(this, stateMachine, "isAttacking", firePoint, enemyData, this);
        deadState = new E2_DeadState(this, stateMachine, "isDead", enemyData, this);
        stuntState = new E2_StuntState(this, stateMachine, "isStunned", enemyData, this);

        stateMachine.Initialize(moveState);

    }
    public void ShootProjectileAtTarget()
    {
        if (currentTarget == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(currentTarget.transform, enemyDataSO.damage);
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Show AOE radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, enemyData.longRange);
    }
}
