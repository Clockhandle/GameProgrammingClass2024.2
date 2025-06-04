using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailDamageEffect : MonoBehaviour
{
    public float speed = 5f;
    public float duration = 2f;
    public int damage = 10;
    private LayerMask unitLayer =>LayerMask.GetMask("Unit");

    private Vector2 moveDirection;
    private float timer;
    private HashSet<Unit> damagedUnits = new HashSet<Unit>();


   
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
            return;
        }
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & unitLayer) != 0)
        {
            Unit unit = collision.GetComponent<Unit>();
            if (unit != null && !damagedUnits.Contains(unit))
            {
                unit.TakeDamage(damage);
                damagedUnits.Add(unit);
            }
        }
    }





}
