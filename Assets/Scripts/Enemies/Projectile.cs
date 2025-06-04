using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    protected int damage;
    private float speed = 10f;

    public virtual void Initialize(Transform target, int damage)
    {
        this.target = target;
        this.damage = damage;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    protected virtual  void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Unit")) return;

        Unit unit = other.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(damage);
            Debug.Log("Projectile HITTTT BITCHHH");
        }

        Destroy(gameObject);
    }
}
