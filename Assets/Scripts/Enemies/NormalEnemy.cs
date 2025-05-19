using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    public Vector2 moveDirection = Vector2.left;

    protected override void OnUpdate()
    {
        // Example: simple straight movement
        transform.Translate(moveDirection.normalized * enemyData.moveSpeed * Time.deltaTime);
    }
}
