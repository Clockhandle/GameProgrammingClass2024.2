using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_MovingImpale : MeleeAttackState
{
    private Enemy3 enemy3;
    private int attackCount = 0;
    public GameObject pierceColliderPrefab;
    public E3_MovingImpale(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, EnemyDataSO enemyData, GameObject pierceColliderPrefab, Enemy3 enemy3) : base(entity, stateMachine, animBoolName, attackPosition, enemyData)
    {
        this.enemy3 = enemy3;
        this.pierceColliderPrefab = pierceColliderPrefab;
    }


}
