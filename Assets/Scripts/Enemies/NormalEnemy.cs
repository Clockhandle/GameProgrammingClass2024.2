using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    public int debugCurrentHealth;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (currentTarget == null && other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null)
            {
                SetTarget(unit);
                Debug.Log("Enemy detected unit: " + unit.name);
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        debugCurrentHealth = currentHealth; // show health in Inspector
    }
}
