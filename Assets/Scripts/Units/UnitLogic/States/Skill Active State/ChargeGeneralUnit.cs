using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGeneralUnit : Unit
{
    [Header("Charge Skill")]
    [SerializeField]  float healthDamageMultiplier = 0.34f;

    private bool isOnCooldown = false;
    private bool chargeNextAttack = false;

    public void ActiveChargeSkill()
    {
        if (isOnCooldown) return;

        isOnCooldown = true;
        chargeNextAttack = true;
        animator.SetTrigger("Charging");

        StartCoroutine(CooldownRoutine());
    }

    public void TriggerChargeDamage()
    {
        if (CurrentTarget == null) return;

        int damageToApply = unitDataSO.attackDamage;

        if (chargeNextAttack)
        {
            int extraDamage = (int)(CurrentTarget.GetComponent<Entity>().enemyDataSO.maxHealth * healthDamageMultiplier);
            damageToApply += extraDamage;
            chargeNextAttack = false;
        }

        CurrentTarget.GetComponent<Entity>().TakeDamage(damageToApply);
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(unitDataSO.skillCoolDown);
        isOnCooldown = false;
    }

}
