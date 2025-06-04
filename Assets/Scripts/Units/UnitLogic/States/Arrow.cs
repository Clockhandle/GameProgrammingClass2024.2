using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform target;
    private int damage;
    private float speed = 10f;

    public void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, 3f); // auto-cleanup
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Optionally rotate arrow to face direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only respond to GameObjects tagged "Enemy"
        if (!collision.CompareTag("Enemy"))
            return;

        Debug.Log($"Arrow hit {collision.name}");

        var enemy = collision.GetComponent<Entity>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject); // Disappear after hitting enemy
    }


}
