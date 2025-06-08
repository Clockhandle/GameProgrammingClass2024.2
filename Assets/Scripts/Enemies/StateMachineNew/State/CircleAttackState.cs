using UnityEngine;

public class CircleAOEDamage : MonoBehaviour
{
    public float duration = 0.5f;
    public int damage = 10;
    public LayerMask targetLayer;
    public float radius = 1.5f;

    void Start()
    {
        // Immediately apply damage
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Unit unit))
            {
                unit.TakeDamage(damage);
            }
        }

        Destroy(gameObject, duration); // Remove after time
    }

    void OnDrawGizmosSelected()
    {
        // Draw circle radius in Scene view
        Gizmos.color = new Color(1, 0, 0, 0.4f); // Red with transparency
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
