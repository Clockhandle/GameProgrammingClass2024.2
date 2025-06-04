using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntTrigger : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();    
    }

    private void Start()
    {
        StunAllNearbyEnemies();

        Invoke(nameof(RemoveObj), 2f);
    }

    private void RemoveObj()
    {
        gameObject.SetActive(false);
    }

    private void StunAllNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.0f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Entity>(out var entity))
            {
                if (entity is Enemy1 enemy1)
                {
                    enemy1.stateMachine.ChangeState(enemy1.stuntState);
                    Debug.Log("Enemy1 stunned");
                }
                else if (entity is Enemy2 enemy2)
                {
                    enemy2.stateMachine.ChangeState(enemy2.stuntState);
                    Debug.Log("Enemy2 stunned");
                }
            }
        }
    }
}
