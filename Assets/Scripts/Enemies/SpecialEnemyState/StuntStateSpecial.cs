using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntStateSpecial : SpecialEnemyState
{
    private float stunDuration = 2f;
    private float timer = 0f;

    public void SetStunDuration(float duration)
    {
        stunDuration = duration;
    }

    public override void EnterState()
    {
        timer = 0f;
        enemy.animator_refe.SetBool("isStunned", true);

        // Optionally stop movement
        var pathFollower = enemy.GetComponent<EnemyPathFollower>();
        if (pathFollower != null)
        {
            pathFollower.isPaused = true;
        }
        enemy.animator_refe.SetBool("isAttacking", false); // cancel the attack animation
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer >= stunDuration)
        {
            stateMachine.ChangeState(enemy.GetComponent<WalkStateSpecial>());
        }
    }

    public override void ExitState()
    {
        enemy.animator_refe.SetBool("isStunned", false);

        // Re-enable movement
        var pathFollower = enemy.GetComponent<EnemyPathFollower>();

        if (pathFollower != null)
        {
            pathFollower.isPaused = false;
        }
    }
}


// Change from outside, call this shit
//EnemyStunState stunState = enemy.GetComponent<EnemyStunState>();
//if (stunState != null)
//{
//    stunState.SetStunDuration(3f); // Optional: customize duration
//    enemy.StateMachine.ChangeState(stunState);
//}