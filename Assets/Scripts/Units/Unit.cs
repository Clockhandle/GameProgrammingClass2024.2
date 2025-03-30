using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitDataSO unitDataSO;
    private UnitStates currentStates;
    private void Start()
    {
        currentStates = UnitStateFactory.CreateState(unitDataSO);
        currentStates?.StartState(this);
    }

    private void Update()
    {
        currentStates?.UpdateState(this);
    }

    //Maybe if we can upgrade an unit to change class, other wise don't need
    public void SwitchState(UnitStates newState)
    {
        currentStates?.ExitState(this);
        currentStates = newState;
        currentStates?.StartState(this);
    }
}
