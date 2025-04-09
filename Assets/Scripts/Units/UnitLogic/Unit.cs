using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitDataSO unitDataSO;
    private UnitStates currentStates;
    //private void Start()
    //{
    //    currentStates = UnitStateFactory.CreateState(unitDataSO);
    //    currentStates?.StartState(this);
    //}

    //Might add animation here, or in each units respective class, idk

    public void InitializeAwaitDeploymentState()
    {
        currentStates = new UnitAwaitDeploymentState();
        currentStates?.StartState(this);
        Debug.Log($"{gameObject.name} initialized state to UnitAwaitDeploymentState");

        //Disable unwanted components here
    }

    public void ConfirmPlacement(Quaternion finalRotation)
    {
        if(currentStates is UnitAwaitDeploymentState)
        {
            transform.rotation = finalRotation;
            UnitStates deployedStates = UnitStateFactory.CreateState(unitDataSO);
            SwitchState(deployedStates);

            //Do stuff here (animation, return image to normal opacity, etc)

            Debug.Log($"{gameObject.name} confirmed deployment facing {finalRotation.eulerAngles} and states {deployedStates.GetType().Name}");
        }
        else
        {
            Debug.Log("ConfirmPlacement not triggered as current state is not set to UnitAwaitDeploymentState");
        }
    }

    public void InitiateRetreat()
    {
        if(currentStates != null)
        {
            Debug.Log($"Retreat Unit {gameObject.name}");
            if (TileManager.Instance != null && TileManager.Instance.tileOccupancyCheck != null)
            {
                // Use the overload taking world position
                TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(transform.position, false);
            }
            else { Debug.LogError($"Cannot unoccupy tile for {gameObject.name}, TileManager or OccupancyCheck reference missing!"); }

            //Phase 2: Retreatment Logic

            //The retreat is done at any point in time, though with a few differences.
            //In UnitAwaitDeploymentState, the UI belonging to said unit is shown, with the addition of a drag direction ui.
            //That means both the UI for retreat and skills are available.

            //When unit is already deployed, all the UI will only be triggered once player pressed on said unit.
            //Though we would like to customize what should be shown and what shouldn't, so something that allows said freedom would be ideal.

            //Retreat will put the Unit into a cooldown state, it needs to pass a certain amount of time before the unit is available for deployment again.
            //Though no need to concern with the actual logic for now, just a state to indicate that and a rough timer is enough.

            //After cooldown, update SO of unit so that they will regain deployment numbers, etc every other logic before players dragged said unit for deployment.

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Initialized retreat function failed. currentStates is null");
        }
    }
    private void Update()
    {
        currentStates?.UpdateState(this);
    }


    public void SwitchState(UnitStates newState)
    {
        currentStates?.ExitState(this);
        currentStates = newState;
        currentStates?.StartState(this);
    }
}
