using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAwaitDeploymentState : UnitStates
{
    public override void StartState(Unit unit)
    {
        base.StartState(unit);
        Debug.Log($"{unit.gameObject.name} entered awaiting deployment stage");
    }

    public override void UpdateState(Unit unit) 
    {
        base.UpdateState(unit); 
    }

    public override void ExitState(Unit unit)
    {
        base.ExitState(unit);
        Debug.Log($"{unit.gameObject.name} exited awaiting deployment stage");
    }
}