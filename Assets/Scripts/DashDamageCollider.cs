using UnityEngine;

public class DashDamageCollider : MonoBehaviour
{
    [SerializeField] private int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Entity>(out Entity enemy))
        {
            Debug.Log("DASHING DADMAGEEE");
            enemy.TakeDamage(damage);
        }
    }
}
