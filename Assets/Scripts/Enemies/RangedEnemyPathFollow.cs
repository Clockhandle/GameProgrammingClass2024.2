using UnityEngine;

public class RangedEnemyPathFollower : EnemyPathFollower
{
    private EnemyRanged rangedEnemy;
    private float walkDuration = 2f;
    private float walkTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        rangedEnemy = GetComponent<EnemyRanged>();
    }

    protected override void Update()
    {
        if (rangedEnemy == null)
        {
            base.Update();
            return;
        }

        // Check if target is in range
        if (rangedEnemy.HasTargetInRange())
        {
            if (!rangedEnemy.isAttacking && !rangedEnemy.isWalkingPhase)
            {
                // Start the shooting animation
                rangedEnemy.TriggerAttack();
            }

            if (rangedEnemy.isWalkingPhase)
            {
                walkTimer += Time.deltaTime;
                base.Update(); // allow movement

                if (walkTimer >= walkDuration)
                {
                    walkTimer = 0f;
                    rangedEnemy.isWalkingPhase = false; // ready to shoot again
                }
            }

            // If isAttacking: do not move, wait for animation to end
            return;
        }

        // No target: resume normal movement
        rangedEnemy.Animator.SetBool("isAttacking", false);
        rangedEnemy.Animator.SetBool("isWalking", true);
        rangedEnemy.isAttacking = false;
        rangedEnemy.isWalkingPhase = false;
        base.Update();
    }
}
