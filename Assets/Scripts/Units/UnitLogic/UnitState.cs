using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState
{
    protected Unit unit;
    protected UnitStateMachine stateMachine;

    public UnitState(Unit unit, UnitStateMachine stateMachine)
    {
        this.unit = unit;
        this.stateMachine = stateMachine;
    }
    public virtual void Enter()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void Exit()
    {

    }
}