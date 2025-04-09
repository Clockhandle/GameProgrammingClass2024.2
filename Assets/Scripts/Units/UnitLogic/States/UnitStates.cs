using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStates
{
    void StartState(Unit unit);
    void UpdateState(Unit unit);
    void ExitState(Unit unit);
}

public abstract class UnitStates : IUnitStates
{
    public virtual void StartState(Unit unit) { }
    public virtual void UpdateState(Unit unit) { }
    public virtual void ExitState(Unit unit) { }
}
