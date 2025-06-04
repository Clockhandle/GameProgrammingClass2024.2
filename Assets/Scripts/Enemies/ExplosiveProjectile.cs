using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    public float explosionRadius = 2f;
    private bool hasExploded = false;

    [SerializeField] private Animator animator;
    [SerializeField] private string explosionTriggerName = "Explode";

    public override void Initialize(Transform target, int damage)
    {
        base.Initialize(target, damage+2);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        if (!other.CompareTag("Unit")) return;

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        Collider2D[] hitUnits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Unit"));
        foreach (Collider2D collider in hitUnits)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
            }
        }

        if (animator != null)
        {
            animator.SetTrigger(explosionTriggerName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnExplosionComplete()  // Put in animation event end
    {
        Destroy(gameObject);
    }
}