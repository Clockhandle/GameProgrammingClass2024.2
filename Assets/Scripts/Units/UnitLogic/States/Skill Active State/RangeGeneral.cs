using System.Collections;
using UnityEngine;

public class UnitRangeGeneral : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;

    private Unit unit;
    private Animator animator;

    [SerializeField] bool isBurstActive = false;
    private float originalAnimatorSpeed = 1f;
    private float burstSpeedMultiplier = 1.5f; // Additive multiplier (so final speed = original + multiplier)
    private float burstDuration = 5f;

    public void Initialize(Unit unit)
    {
        this.unit = unit;
        this.animator = unit.GetComponent<Animator>();

        if (arrowSpawnPoint == null)
            arrowSpawnPoint = this.transform;

        if (arrowPrefab == null && unit.ArrowPrefab != null)
            arrowPrefab = unit.ArrowPrefab;

        if (animator != null)
            originalAnimatorSpeed = animator.speed;
    }

    public void ActivateBurstSkill()
    {
        if (isBurstActive || animator == null) return;

        StartCoroutine(BurstSkillRoutine());
    }

    private IEnumerator BurstSkillRoutine()
    {
        isBurstActive = true;

        // Increase animator speed
        animator.speed *= burstSpeedMultiplier;

        // Optional: Add VFX, SFX, or feedback here

        yield return new WaitForSeconds(burstDuration);

        // Reset animator speed to original
        animator.speed = originalAnimatorSpeed;
        isBurstActive = false;
    }

    // Called by animation event
    public void FireArrowAnimationEvent()
    {
        if (!unit.IsOperational || unit.enemiesInRangeList.Count == 0)
            return;

      
        // Burst: fire at all enemies
        if (isBurstActive)
        {
            foreach (var enemy in unit.enemiesInRangeList)
            {
                if (enemy == null) continue;

                GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
                Arrow arrow = arrowObj.GetComponent<Arrow>();

                if (arrow != null)
                {
                    int damage = unit.unitDataSO != null ? unit.unitDataSO.attackDamage : 1;
                    arrow.Initialize(enemy.transform, damage);
                }
            }
        }
        // Normal: fire at first enemy only
        else
        {
            var target = unit.enemiesInRangeList[0];
            if (target == null) return;

            GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
            Arrow arrow = arrowObj.GetComponent<Arrow>();

            if (arrow != null)
            {
                int damage = unit.unitDataSO != null ? unit.unitDataSO.attackDamage : 1;
                arrow.Initialize(target.transform, damage);
            }
        }
    }
}
