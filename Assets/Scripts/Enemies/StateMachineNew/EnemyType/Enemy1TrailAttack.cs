using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1TrailAttack : Enemy1
{

    public GameObject trailEffectPrefab;
    public GameObject additionEffect;
    private EnemyDirectionHandler directionHandler;


    public override void Start()
    {
        base.Start();

        directionHandler = GetComponent<EnemyDirectionHandler>();

        melleAttackState = new E1_TrailAttackState(this, stateMachine, "isAttacking", attackPosition, enemyData, this);

        stateMachine.Initialize(moveState);
    }

    public void SpawnTrailAttackEffect()
    {
        if (trailEffectPrefab != null && attackPosition != null)
        {
            GameObject trail = Instantiate(trailEffectPrefab, attackPosition.position, Quaternion.identity);
            additionEffect.SetActive(true);
            Invoke(nameof(SEtActiveAddition), .5f);


            TrailDamageEffect trailScript = trail.GetComponent<TrailDamageEffect>();
            if (trailScript != null && directionHandler != null)
            {
                trailScript.SetMoveDirection(GetDirectionFromFacing(directionHandler.currentDirection));
            }
        }
    }
    void SEtActiveAddition()
    {
        additionEffect.SetActive(false);
    }

    private Vector2 GetDirectionFromFacing(EnemyDirectionHandler.FacingDirection dir)
    {
        return dir switch
        {
            EnemyDirectionHandler.FacingDirection.Up => Vector2.up,
            EnemyDirectionHandler.FacingDirection.Down => Vector2.down,
            EnemyDirectionHandler.FacingDirection.Left => Vector2.left,
            EnemyDirectionHandler.FacingDirection.Right => Vector2.right,
            _ => Vector2.right
        };
    }
    public override void Update()
    {
        base.Update();
    }
}
