using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEProjectile : MonoBehaviour
{
    public float speed = 10f;
    public GameObject hitEffectPrefab;

    private Vector2 startPos;
    private Vector2 targetPos;
   
    private float heightArc = 3f;

    [Header("AOE setting")]
    public int aoeRadius = 2;
    public int damage;
    public LayerMask unitLayer => LayerMask.GetMask("Unit");
    
    public void Initialize(Vector2 start, Vector2 target, int damage)
    {
        this.startPos = start;
        this.targetPos = target;
        this.damage = damage;
        transform.position = start;

        StartCoroutine(MoveArc());
    }

    private IEnumerator MoveArc()
    {
  
        float time = 0;
        float duration = Vector2.Distance(startPos, targetPos) / speed;

        while (time < duration)
        {
            float t = time / duration;

            // Parabolic arc
            Vector2 pos = Vector2.Lerp(startPos, targetPos  , t);
            float height = Mathf.Sin(t * Mathf.PI) * heightArc;
            transform.position = new Vector3(pos.x, pos.y + height, 0);

            time += Time.deltaTime;
            yield return null;
        }

        // Hit ground
        OnImpact();
    }


    private void OnImpact()
    {
        if (hitEffectPrefab)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, targetPos, Quaternion.identity);
            Destroy(hitEffect, 2f); // Destroy after 2 seconds
        }

        // Do AoE damage
        Collider2D[] hitUnits = Physics2D.OverlapCircleAll(targetPos, aoeRadius, unitLayer);

        foreach (Collider2D collider in hitUnits)
        {
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(damage);
                Debug.Log("AOE hit: " + unit.name);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Show AOE radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPos, aoeRadius);
    }
}
// NEED TO HANDLE THE DAMAGE DEALT 
