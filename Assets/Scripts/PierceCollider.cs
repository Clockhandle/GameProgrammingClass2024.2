using UnityEngine;

public class PierceCollider : MonoBehaviour
{
    public float duration = 0.2f;
    private int damage;
    private LayerMask targetLayer;
    private System.Action<bool> onHit;

    public void Initialize(int damage, LayerMask targetLayer, System.Action<bool> onHit)
    {
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.onHit = onHit;

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        bool hitUnit = false;
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(12, 3), 0, targetLayer);

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
        Gizmos.DrawWireCube(transform.position, new Vector3(12, 3, 0));
    }
}
