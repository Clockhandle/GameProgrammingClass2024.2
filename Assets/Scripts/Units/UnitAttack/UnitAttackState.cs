using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackState : UnitStates
{
    private float timeSinceLastAttack;
    private bool isAttacking;

    public override void StartState(Unit unit)
    {
        base.StartState(unit);
        timeSinceLastAttack = 0f;
        isAttacking = false;
    }

    public override void UpdateState(Unit unit)
    {
        base.UpdateState(unit);
        timeSinceLastAttack += Time.deltaTime;
        if (!isAttacking && timeSinceLastAttack >= unit.unitDataSO.attackInterval)
        {
            // Detect enemies in range BEFORE triggering the attack animation
            Vector2 direction = unit.facingRight ? Vector2.right : Vector2.left;
            Vector2 center = (Vector2)unit.transform.position + direction * unit.unitDataSO.attackOffset.x;
            float radius = unit.unitDataSO.attackRange;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius, LayerMask.GetMask("Enemy"));

            if (hitColliders.Length > 0)
            {
                isAttacking = true;
                unit.animator.SetTrigger("Attack");
            }
        }
    }


    public override void ExitState(Unit unit)
    {
        base.ExitState(unit);
        isAttacking = false;
    }

    public void PerformAttack(Unit unit)
    {
        Vector2 direction = unit.facingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)unit.transform.position + direction * unit.unitDataSO.attackOffset.x;
        float radius = unit.unitDataSO.attackRange;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius, LayerMask.GetMask("Enemy"));
        Debug.Log($"Detected {hitColliders.Length} enemies");
        foreach (var hit in hitColliders)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                Debug.Log($"Attacked enemy: {enemy.name}");
                enemy.TakeDamage(unit.unitDataSO.attackDamage);
            }
        }

        timeSinceLastAttack = 0f;
        isAttacking = false;
    }

}
