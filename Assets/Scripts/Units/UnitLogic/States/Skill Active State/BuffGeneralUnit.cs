using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffGeneralUnit : Unit
{
    [Header("Buff Skill Settings")]
    [SerializeField] private float buffRadius = 5f;
    [SerializeField] private int bonusHealth = 30;
    [SerializeField] private int bonusDamage = 10;
    [SerializeField] private float animSpeedMultiplier = 1.5f;
    [SerializeField] private float buffDuration = 5f;


    private bool skillUsed = false;

    private class BuffGeneralUnitInfo
    {
        public Unit unit;
        public int originalDamage;
        public float originalAnimSpeed;
    }

    public void ActivateBuffSkill()
    {
        if ( skillUsed) return;

        skillUsed = true;
        Debug.Log("Buff SKILL ACTIVE");
        StartCoroutine(ApplyTemporaryBuff());
    }

    private IEnumerator ApplyTemporaryBuff()
    {
        List<BuffGeneralUnitInfo> buffedAllies = new List<BuffGeneralUnitInfo>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, buffRadius);
        foreach (var hit in hits)
        {
            Unit ally = hit.GetComponent<Unit>();
            if (ally != null && ally != this && ally.IsOperational)
            {
                var info = new BuffGeneralUnitInfo
                {
                    unit = ally,
                    originalDamage = ally.unitDataSO.attackDamage,
                    originalAnimSpeed = ally.animator != null ? ally.animator.speed : 1f
                };

                // Increase health (heals but does not raise max)
                ally.currentHealth += bonusHealth;
                ally.currentHealth = Mathf.Min(ally.currentHealth, ally.MaxHealth);

                // Increase attack damage (temporary)
                ally.unitDataSO.attackDamage += bonusDamage;

                // Increase animation speed
                if (ally.animator != null)
                {
                    ally.animator.speed *= animSpeedMultiplier;
                }

                buffedAllies.Add(info);
            }
        }

        Debug.Log("Buff applied to " + buffedAllies.Count + " allies.");
        yield return new WaitForSeconds(buffDuration);

        // Revert buffs
        foreach (var allyInfo in buffedAllies)
        {
            if (allyInfo.unit != null)
            {
                allyInfo.unit.unitDataSO.attackDamage = allyInfo.originalDamage;

                if (allyInfo.unit.animator != null)
                {
                    allyInfo.unit.animator.speed = allyInfo.originalAnimSpeed;
                }

                Debug.Log($"{allyInfo.unit.name}'s buff reverted.");
            }
        }

        Debug.Log("All buffs reverted.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }

}

