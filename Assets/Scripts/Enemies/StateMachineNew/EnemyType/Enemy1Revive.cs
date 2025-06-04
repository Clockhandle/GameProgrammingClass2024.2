using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Revive : Enemy1
{
    public E1_ReviveState reviveState { get; private set; }

    public bool hasRevived = false;
 

    [SerializeField] private RuntimeAnimatorController normalController;
    [SerializeField] private RuntimeAnimatorController revivedController;

    public override void Start()
    {
        base.Start();

        reviveState = new E1_ReviveState(this, stateMachine, "isReviving", enemyData, this);

        anim.runtimeAnimatorController = normalController; // start with normal animations
    }

    public override void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 2 && !hasRevived)
        {
            currentHealth = 2; // clamp HP to 1 before revive
            hasRevived = true;
            isDead = true; // temporarily dead for revive state
            stateMachine.ChangeState(reviveState);
            return;
        }

        if (currentHealth <= 0 && hasRevived)
        {
            isDead = true;
            stateMachine.ChangeState(deadState);
           
        }
    }

    public void ResetHealth()
    {
        currentHealth = enemyData.maxHealth;
        isDead = false;
    }

    public void SetReviveEffects()
    {
        anim.runtimeAnimatorController = revivedController;
    }

 

}
