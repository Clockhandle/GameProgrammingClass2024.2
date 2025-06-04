using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackStateSpecial : SpecialEnemyState
{
    public override void EnterState()
    {
        Debug.Log("Special Skill Activated!");
        // Trigger special attack animation
    }

    public override void UpdateState()
    {
        // Logic to decide when to exit
    }

    public override void ExitState()
    {
        // Cleanup
    }
}
