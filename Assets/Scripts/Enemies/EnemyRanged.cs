using UnityEngine;

public class EnemyRanged : EnemyBase
{
    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    private Transform firePoint;
    public float attackRange = 4f;

    [Header("Attack Logic")]
    private float phaseDuration = 2f;
    private float phaseTimer = 0f;

    public bool isAttacking = false;
    public bool isWalkingPhase = false;

    protected override void OnUpdate()
    {
        firePoint = this.transform;

        if (isDead || projectilePrefab == null || firePoint == null)
            return;

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                phaseTimer += Time.deltaTime;

                if (isAttacking)
                {
                    animator.SetBool("isAttacking", true);
                    // wait for animation to call FireProjectileAnimationEvent()
                }
                else
                {
                    animator.SetBool("isAttacking", false);
                  //  WalkForward();
                }

                if (phaseTimer >= phaseDuration)
                {
                    phaseTimer = 0f;
                    isAttacking = !isAttacking;
                }
            }
            else
            {
                animator.SetBool("isAttacking", false);
              
            }
        }
        else
        {
            animator.SetBool("isAttacking", false);
            FindNearestTargetInRange();
        }
    }

    //private void WalkForward()
    //{
    //    transform.position += transform.right * Time.deltaTime * 1.5f; // walk speed
    //}

    private void FindNearestTargetInRange()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Unit"))
            {
                Unit unit = hit.GetComponent<Unit>();
                if (unit != null)
                {
                    SetTarget(unit);
                    break;
                }
            }
        }
    }
    public bool HasTargetInRange()
    {
        if (isDead || currentTarget == null) return false;
        float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
        return distance <= attackRange;
    }
    public void TriggerAttack()
    {
        if (!isDead && currentTarget != null)
        {
            isAttacking = true;
            isWalkingPhase = false;
            animator.SetBool("isAttacking", true);
            animator.SetBool("isWalking", false);
        }
    }
    public void EndAttackAndStartWalkingPhase()
    {
        isAttacking = false;
        isWalkingPhase = true;
        animator.SetBool("isAttacking", false);
        animator.SetBool("isWalking", true);
    }

    public void FireProjectileAnimationEvent()
    {
        if (currentTarget == null || !HasTargetInRange()) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(currentTarget.transform, enemyData.damage);
        }

        // After firing, switch to walking
        EndAttackAndStartWalkingPhase();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
