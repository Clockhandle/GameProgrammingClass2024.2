using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyMover : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.left;
    public float offsetRadius = 0.2f;

    private bool isBlocked = false;
    private Vector3 blockOffset = Vector3.zero;
    private Vector3 blockBasePosition = Vector3.zero;
    private Unit blockingUnit = null;
    private EnemyBase enemyBase;

    void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
    }

    void Update()
    {
        if (!isBlocked)
        {
            float speed = enemyBase != null && enemyBase.enemyData != null
                ? enemyBase.enemyData.moveSpeed
                : 2f;
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime);
        }
        else
        {
            transform.position = blockBasePosition + blockOffset;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (!isBlocked && unit != null && unit.IsOperational && unit.CanBlockMore())
        {
            isBlocked = true;
            blockBasePosition = transform.position;
            blockOffset = (Vector3)(Random.insideUnitCircle * offsetRadius);
            blockingUnit = unit;
            unit.AddBlockedEnemy(this.GetComponent<EnemyMover>());
        }

        if (other.CompareTag("Goal"))
        {
            GameManager.Instance?.UnregisterEnemy(gameObject);
            GameManager.Instance?.OnEnemyReachedGoal();
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
            unit.RemoveBlockedEnemy(this.GetComponent<EnemyMover>());
            blockingUnit = null;
        }
    }
}