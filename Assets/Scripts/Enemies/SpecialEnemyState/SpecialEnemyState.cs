using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialEnemyState : MonoBehaviour
{
    protected SpecialEnemyStateMachine stateMachine;
    protected EnemyBase enemy;
   


    public void Initialize(SpecialEnemyStateMachine stateMachine, EnemyBase enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void OnTriggerEnter2D(Collider2D other) { }
}
