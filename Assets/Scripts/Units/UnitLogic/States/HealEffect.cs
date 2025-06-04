using UnityEngine;

public class HealEffect : MonoBehaviour
{
    private Transform target;
    private int healAmount;
    public float speed = 5f;

    public void Initialize(Transform target, int healAmount)
    {
        this.target = target;
        this.healAmount = healAmount;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // cant use overlap cause it can hit other collider on its way
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Unit unit = target.GetComponent<Unit>();
            if (unit != null)
            {
                unit.Heal(healAmount);
            }
            Destroy(gameObject);
        }
    }
}
