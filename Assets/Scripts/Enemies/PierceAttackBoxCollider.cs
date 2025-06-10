using UnityEngine;

public class PierceAttackBoxCollider : MonoBehaviour
{
    public float duration = 0.2f;
    private int damage;
    private LayerMask targetLayer;
    private System.Action<bool> onHit;
    public Vector2 customCollider;

    public void Initialize(int damage, LayerMask targetLayer, Vector2 customCollider, System.Action<bool> onHit)
    {
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.onHit = onHit;
        this.customCollider = customCollider;


        ApplyDamage();
    }

    private void ApplyDamage()
    {
        bool hitUnit = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, customCollider, 0, targetLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Unit unit))
            {
                unit.TakeDamage(damage);
                hitUnit = true;
            }
        }

        onHit?.Invoke(hitUnit);
        Destroy(gameObject, duration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, customCollider);
    }
}
