using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector2 moveDirection = Vector2.left;
    public float moveSpeed = 2f;

    [Header("Block Offset Settings")]
    public float offsetRadius = 0.2f;

    private bool isBlocked = false;
    private Vector3 blockOffset = Vector3.zero;
    private Vector3 blockBasePosition = Vector3.zero;
    private Unit blockingUnit = null;

    void Update()
    {
        if (!isBlocked)
        {
            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = blockBasePosition + blockOffset;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isBlocked)
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null && unit.CanBlockMore())
            {
                isBlocked = true;
                blockBasePosition = transform.position;
                blockOffset = (Vector3)(Random.insideUnitCircle * offsetRadius);
                blockingUnit = unit;
                unit.AddBlockedEnemy(this);
            }
        }

        if (other.CompareTag("Goal"))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (isBlocked && unit != null && unit == blockingUnit)
        {
            isBlocked = false;
            blockOffset = Vector3.zero;
            unit.RemoveBlockedEnemy(this);
            blockingUnit = null;
        }
    }
}