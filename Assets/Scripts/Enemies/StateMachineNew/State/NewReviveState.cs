using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewReviveState : State
{
    protected EnemyDataSO revivedData;
    protected RuntimeAnimatorController revivedAnimator;
    protected float reviveDelay = 4f;
    protected string reviveTrigger = "isReviving";

    public NewReviveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO revivedData, RuntimeAnimatorController revivedAnimator) : base(entity, stateMachine, animBoolName)
    {
        this.revivedAnimator = revivedAnimator;
        this.revivedData = revivedData;
    }

    public override void Enter()
    {
        base.Enter();
        entity.Move(false);
        entity.anim.SetTrigger(reviveTrigger);
        entity.ResetHealth(revivedData.maxHealth);

        entity.StartCoroutine(ReviveCoroutine());
    }

    protected virtual IEnumerator ReviveCoroutine()
    {
        yield return new WaitForSeconds(reviveDelay);

        // Boost stats and change animator
        entity.enemyDataSO = revivedData;
        entity.anim.runtimeAnimatorController = revivedAnimator;

        entity.Move(true);

        OnReviveComplete();
    }

    protected virtual void OnReviveComplete()
    {
        // Override this in subclasses
    }
}
