using UnityEngine;
using System.Collections.Generic;


public class EnemyRanged : EnemyBase
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 4f;
    public LayerMask unitLayerMask;


    public bool isAttacking = false;
    public bool isWalking = true;

    [Header("Attack Phases")]
    public float walkAfterAttackDuration = 4f;
    private float walkAfterAttackTimer = 0f;
    private bool isWalkingAfterAttack = false;


    public List<Unit> detectedUnits = new List<Unit>();

    protected override void Update()
    {

        firePoint = this.transform;
        if (isWalkingAfterAttack)
        {
            walkAfterAttackTimer += Time.deltaTime;
            TriggerWalk();
            if (walkAfterAttackTimer >= walkAfterAttackDuration)
            {
                walkAfterAttackTimer = 0f;
                isWalkingAfterAttack = false;

                if (HasUnitInRange())
                {
                    TriggerAttack(); // Attack again after walking
                }
                else
                {
                    TriggerWalk();
                    currentTarget = null;
                }
            }

            return;

        }

        if (HasUnitInRange())
        {
            if (!isAttacking)
            {
                TriggerAttack(); // Attack immediately
            }
        }
        else
        {
            // No target in range
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
            isAttacking = false;
            isWalking = true;
            currentTarget = null;
        }
    }

    public bool HasUnitInRange()
    {
        detectedUnits.Clear();
        currentTarget = null;

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        float closestDist = float.MaxValue;

        foreach (var unitGO in units)
        {
            Unit unit = unitGO.GetComponent<Unit>();
            if (unit == null || unit.isDead) continue; 

            float dist = Vector2.Distance(transform.position, unitGO.transform.position);
            if (dist <= attackRange)
            {
                detectedUnits.Add(unit);

                if (dist < closestDist)
                {
                    currentTarget = unit;
                    closestDist = dist;
                }
            }
        }

        return currentTarget != null;
    }

    public void TriggerAttack()
    {
        isAttacking = true;
        isWalking = false;
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
    }
    public void TriggerWalk()
    {
        isAttacking = false;
        isWalking = true;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
    }

    public void FireProjectileAnimationEvent()
    {
        if (currentTarget == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(currentTarget.transform, enemyData.damage);
        }

        // Start walking phase after attack
        //isAttacking = false;
        //isWalking = true;
        isWalkingAfterAttack = true;
        walkAfterAttackTimer = 0f;

        //animator.SetBool("isAttacking", false);
        //animator.SetBool("isWalking", true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
