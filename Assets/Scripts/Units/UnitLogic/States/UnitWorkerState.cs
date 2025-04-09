using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitWorkerState : UnitStates
{

    public override void StartState(Unit unit)
    {
        base.StartState(unit);
        Debug.Log($"Hello from {GetType()} and {unit.name}, test your shit here");
    }

    public override void UpdateState(Unit unit)
    {
        base.UpdateState(unit);
    }
    public override void ExitState(Unit unit)
    {
        base.ExitState(unit);
    }
}
