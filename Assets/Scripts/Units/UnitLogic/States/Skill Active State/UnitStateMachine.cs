using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateMachine : MonoBehaviour
{
    public UnitState CurrentState { get; private set; }

    public void Initialize(UnitState startingState)
    {
        CurrentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(UnitState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

}
