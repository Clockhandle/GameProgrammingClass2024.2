using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashGeneralUnit : Unit
{
    [Header("Dash Skill Settings")]
    [SerializeField] private GameObject damageColliderPrefab;
    [SerializeField] private Transform colliderSpawnPoint;
    [SerializeField] private float colliderLifetime = 0.7f;

    private bool isOnCooldown = false;

    public void ActivateDashSkill()
    {
        if (isOnCooldown) return;

        isOnCooldown = true;
        animator.SetTrigger("Dashing");

        StartCoroutine(CooldownRoutine());
    }

    public void TriggerDashDamage() // Called from animation event
    {
        if (damageColliderPrefab != null && colliderSpawnPoint != null)
        {
            GameObject col = Instantiate(damageColliderPrefab, colliderSpawnPoint.position, colliderSpawnPoint.rotation);
            Destroy(col, colliderLifetime);
        }
    }

    public void EndDash()  // cal at the end of animation
    {
        animator.SetBool("isDashing", false);
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(unitDataSO.skillCoolDown);
        isOnCooldown = false;
    }

}
