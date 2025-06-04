using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemyStateMachine : MonoBehaviour
{
    private SpecialEnemyState currentState;
    private EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>(); 
    }

    public void Initialize(SpecialEnemyState startingState)
    {
        currentState = startingState;

       
        currentState.Initialize(this, enemy); // Pass in self and enemy

        currentState.EnterState(); 
    }

    public void ChangeState(SpecialEnemyState newState)
    {
        if (currentState != null)
            currentState.ExitState();

        currentState = newState;

        if (currentState != null)
        {
            currentState.Initialize(this, enemy); 
            currentState.EnterState();
        }
    }

    private void Update()
    {
        currentState?.UpdateState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState?.OnTriggerEnter2D(collision);
    }

}
