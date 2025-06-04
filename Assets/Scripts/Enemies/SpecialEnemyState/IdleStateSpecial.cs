using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateSpecial : SpecialEnemyState
{
    
    private float idleDuration = 5f;
    private float timer = 0f;
    private bool hasPaused = false;

    public override void EnterState()
    {
      
        timer = 0f;
        enemy.animator_refe.SetBool("isWalking", false);

        // Stop movement logic
        var pathFollower = enemy.GetComponent<EnemyPathFollower>();
        if (pathFollower != null)
            pathFollower.isPaused = true;
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;
        if (timer >= idleDuration)
        {
            hasPaused = true;

            // Resume path following
            var pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {  
                pathFollower.isPaused = false;
                Debug.Log("Changenign isPausedd IDELELEL");
            }
             

            stateMachine.ChangeState(enemy.GetComponent<WalkStateSpecial>());
        }
    }

    public override void ExitState()
    {
        // Resume walking animation
        enemy.animator_refe.SetBool("isWalking", true);

    }

    public bool HasPausedOnce() => hasPaused;
}
