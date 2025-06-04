using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMedic : MonoBehaviour
{
    [Header("Setup")]
    private Transform healSpawnPoint;
    [SerializeField] private GameObject healEffectPrefab;

    private Unit unit;

    public void Initialize(Unit unit)
    {
        this.unit = unit;
        healSpawnPoint = this.transform;

        if (healEffectPrefab == null && unit.HealEffectPrefab != null)
            healEffectPrefab = unit.HealEffectPrefab;
    }

    void Update()
    {
        // Check if there someone to heal every frame
        if (unit != null && unit.IsOperational)
        {
            Unit target = GetLowestHealthAlly();

            if (target != null)
            {
                unit.TriggerHealingAnim(); // Start healing animation
            }
            else
            {
                unit.animator?.SetBool("isHealing", false); // Return to idle if no one to heal
            }
        }
    }

    public void FireHealAnimationEvent()
    {
        if (!unit.IsOperational || unit.alliesInRangeList.Count == 0)
            return;

        Unit target = GetLowestHealthAlly();
        if (target == null || healEffectPrefab == null || healSpawnPoint == null)
            return;

        GameObject healObj = Instantiate(healEffectPrefab, healSpawnPoint.position, Quaternion.identity);
        HealEffect healEffect = healObj.GetComponent<HealEffect>();

        if (healEffect != null)
        {
            int healAmount = unit.unitDataSO != null ? unit.unitDataSO.healAmount : 5;
            healEffect.Initialize(target.transform, healAmount);
        }
    }

    private Unit GetLowestHealthAlly()
    {
        Unit lowest = null;
        float lowestPercent = 1f;



        foreach (Unit ally in unit.alliesInRangeList)
        {
            if (ally.currentHealth >= ally.unitDataSO.maxHealth)
                continue; // Skip fully healed units

            float percent = (float)ally.currentHealth / ally.MaxHealth;
            if (percent < lowestPercent)
            {
                lowestPercent = percent;
                lowest = ally;
            }
        }

        return lowest;
    }
}
