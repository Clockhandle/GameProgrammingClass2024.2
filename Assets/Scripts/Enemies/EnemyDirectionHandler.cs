using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyDirectionHandler : MonoBehaviour
{
    public enum FacingDirection { Up, Down, Left, Right }
    public FacingDirection currentDirection;

    public enum AttackType { Melee, Ranged }
    public AttackType attackType;

    [Header("Ranged Settings")]
    public Transform firePoint; // Assign in inspector
    public float offsetValue = 1f;

    private CapsuleCollider2D capsuleCollider;
   

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        UpdateBasedOnAttackType();
    }

    void Update()
    {
       
    }


    public void UpdateBasedOnAttackType()
    {
        if (attackType == AttackType.Melee)
        {
            UpdateColliderDirection();
        }
        else if (attackType == AttackType.Ranged)
        {
            UpdateFirePointPosition();
        }
    }

    void UpdateColliderDirection()
    {
        switch (currentDirection)
        {
            case FacingDirection.Up:
            case FacingDirection.Down:
                if(capsuleCollider= null)
                {
                    capsuleCollider.direction = CapsuleDirection2D.Vertical;
                }
                
                break;
            case FacingDirection.Left:
            case FacingDirection.Right:
                if (capsuleCollider = null)
                {
                    capsuleCollider.direction = CapsuleDirection2D.Horizontal;
                }
                break;
        }
    }

    void UpdateFirePointPosition()
    {
        Vector3 offset = Vector3.zero;

        switch (currentDirection)
        {
            case FacingDirection.Up:
                offset = new Vector3(0, offsetValue, 0);
                break;
            case FacingDirection.Down:
                offset = new Vector3(0, -offsetValue, 0);
                break;
            case FacingDirection.Left:
                offset = new Vector3(-offsetValue, 0, 0);
                break;
            case FacingDirection.Right:
                offset = new Vector3(offsetValue, 0, 0);
                break;
        }

        if (firePoint != null)
        {
            firePoint.localPosition = offset;
        }
    }
}

