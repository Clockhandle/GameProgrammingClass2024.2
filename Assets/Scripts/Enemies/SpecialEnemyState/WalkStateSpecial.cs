using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkStateSpecial : SpecialEnemyState
{
    [SerializeField ] private EnemyPathFollower pathFollower;
    private float initialWalkDuration = 1f; 
    private float walkTimer = 0f;
    private bool initialWalkDone = false;

    public override void EnterState()
    {
        walkTimer = 0f;

        enemy.animator_refe.SetBool("isWalking", true);

        var pathFollower = enemy.GetComponent<EnemyPathFollower>();
        if (pathFollower != null)
        {
  pathFollower.isPaused = false;
            Debug.Log("Changenign isPausedd");
        }
          

    }




    public override void UpdateState()
    {

        if (enemy is RangeeEnemy && enemy.TryGetComponent<RangedAttackState>(out var rangedAttackState))
        {
            if (rangedAttackState.HasUnitInRange())
            {
                stateMachine.ChangeState(rangedAttackState);
                return;
            }
        }

        if (enemy.CurrentTarget != null)
        {
            stateMachine.ChangeState(enemy.GetComponent<NormalAttackStateSpecial>());
            return;
        }

        walkTimer += Time.deltaTime;

        if (!initialWalkDone)
        {
            if (walkTimer < initialWalkDuration)
            {
                // Walk briefly after spawn
                pathFollower.FollowPathStep();
            }
            else
            {
                initialWalkDone = true;
                stateMachine.ChangeState(enemy.GetComponent<IdleStateSpecial>()); // go pause for 5s
            }

            return;
        }

        if (initialWalkDone)
        {
            pathFollower.FollowPathStep();
        }
    }

    public override void ExitState()
    {
        enemy.animator_refe.SetBool("isWalking", false);
    }
}
