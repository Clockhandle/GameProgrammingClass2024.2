using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : NormalAttackStateSpecial
{
    [Header("Attack Phases")]
    public float walkAfterAttackDuration = 4f;
    private float walkAfterAttackTimer = 0f;
    private bool isWalkingAfterAttack = false;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 4f;
    public LayerMask unitLayerMask;

    public List<Unit> detectedUnits = new List<Unit>();


    private EnemyPathFollower pathFollower;
    private Entity entity;
    private FiniteStateMachine stateMachine1;
    private string animBoolName;
    private EnemyDataSO enemyData;
    private Enemy2MultiShoot enemy2;

    public RangedAttackState(Entity entity, FiniteStateMachine stateMachine1, string animBoolName, Transform firePoint, EnemyDataSO enemyData, Enemy2MultiShoot enemy2)
    {
        this.entity = entity;
        this.stateMachine1 = stateMachine1;
        this.animBoolName = animBoolName;
        this.firePoint = firePoint;
        this.enemyData = enemyData;
        this.enemy2 = enemy2;
    }

    public override void EnterState()
    {
        base.EnterState();
        enemy = GetComponent<EnemyBase>();
        pathFollower = enemy.GetComponent<EnemyPathFollower>();
        walkAfterAttackTimer = 0f;

        HasUnitInRange();
    }

    public override void UpdateState()
    {
        firePoint = this.transform;

        if (isWalkingAfterAttack)
        {
            walkAfterAttackTimer += Time.deltaTime;
            stateMachine.ChangeState(enemy.GetComponent<WalkStateSpecial>());
            if (walkAfterAttackTimer >= walkAfterAttackDuration)
            {
                walkAfterAttackTimer = 0f;
                isWalkingAfterAttack = false;

                if (HasUnitInRange())
                {
                    stateMachine.ChangeState(enemy.GetComponent<RangedAttackState>()); // Attack again after walking
                }
                else
                {
                    stateMachine.ChangeState(enemy.GetComponent<WalkStateSpecial>());
                    enemy.SetTarget(null);
                }
            }

            return;

        }

        if (HasUnitInRange())
        {
            stateMachine.ChangeState(enemy.GetComponent<RangedAttackState>());
        }
        
   
}



    public bool HasUnitInRange()
    {
        detectedUnits.Clear();
       enemy.SetTarget(null);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, unitLayerMask);
        float closestDist = float.MaxValue;
        Unit closestUnit = null;

        foreach (var hit in hits)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit == null || unit.isDead) continue;

            float dist = Vector2.Distance(transform.position, unit.transform.position);
            detectedUnits.Add(unit);
            Debug.Log("UNIT IS DETECINT GIN RANGE");

            if (dist < closestDist)
            {
                closestUnit = unit;
                closestDist = dist;
            }
        }

        if (closestUnit != null)
        {
            enemy.SetTarget(closestUnit);
            return true;
        }

        return false;
    }


    public void FireProjectileAnimationEvent()
    {
        if (enemy.CurrentTarget == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(enemy.CurrentTarget.transform, enemy.enemyData.damage);
        }

    }
    public override void ExitState()
    {
        base.ExitState();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


}
