using UnityEngine;

public class ExplosiveArrow : Arrow
{
    public float explosionRadius = 2f;
    private bool hasExploded = false;

    [SerializeField] private Animator animator;
    [SerializeField] private string explosionTriggerName = "Explode";

    
    public override void Initialize(Transform target, int damage)
    {
        base.Initialize(target, damage + 7); 
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;
        if (!collision.CompareTag("Enemy")) return;

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider2D collider in hitEnemies)
        {
            Entity enemy = collider.GetComponent<Entity>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
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

    // Call this from the explosion animation event
    public void OnExplosionComplete()
    {
        Destroy(gameObject);
    }

    // Optional: Draw explosion radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
