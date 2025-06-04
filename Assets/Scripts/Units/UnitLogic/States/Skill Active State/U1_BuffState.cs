using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class U1_BuffState : BuffState
{
    public U1_BuffState(Unit unit, UnitStateMachine sm) : base(unit, sm) { }

    public override void Enter()
    {
        Debug.Log("BuffUnit: Entering Buff State");
        // trigger animation, increase stats, etc.
    }

    public override void Update()
    {
        // handle duration, cooldown etc.
    }

    public override void Exit()
    {
        Debug.Log("BuffUnit: Exiting Buff State");
        // cleanup buffs
    }
}
