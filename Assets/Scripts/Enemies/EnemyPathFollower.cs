using UnityEngine;

//[RequireComponent(typeof(EnemyBase))]
public class EnemyPathFollower : MonoBehaviour
{
    public bool isPaused = false;  // paused state


    private EnemyPath path;
    private int currentCheckpoint = 0;
    private float waitTimer = 0f;
    private EnemyBase enemyBase;
    private bool pathComplete = false;

    // Blocking logic
    public float offsetRadius = 0.2f;
    private bool isBlocked = false;
    private Vector3 blockOffset = Vector3.zero;
    private Vector3 blockBasePosition = Vector3.zero;
    private Unit blockingUnit = null;

    //Enemy ranged
    private EnemyRanged ranged;
    private Entity entity;

    public void SetPath(EnemyPath newPath)
    {
        path = newPath;
        currentCheckpoint = 0;
        waitTimer = 0f;
        pathComplete = false;
        Debug.Log($"{gameObject.name}: Path set with {path?.checkpoints.Count ?? 0} checkpoints.");
    }

    protected virtual void Awake()
    {
        //enemyBase = GetComponent<EnemyBase>();
        ranged = GetComponent<EnemyRanged>();
        entity = GetComponent<Entity>();    
    }

   protected virtual void Update()
    {
        if (!isPaused)
        {
            FollowPathStep();
        }
    }

    // Path Follower funcition
    public void FollowPathStep()
    {
        if (isPaused) return; 

        if (ranged != null && !ranged.isWalking)
            return;

        if (isBlocked)
        {
            transform.position = blockBasePosition + blockOffset;
            return;
        }

        if (pathComplete || path == null || path.checkpoints.Count == 0)
        {
            if (path == null)
                Debug.LogWarning($"{gameObject.name}: No path assigned.");
            else if (path.checkpoints.Count == 0)
                Debug.LogWarning($"{gameObject.name}: Path has no checkpoints.");
            return;
        }

        // Defensive: skip null checkpoints
        while (currentCheckpoint < path.checkpoints.Count && path.checkpoints[currentCheckpoint] == null)
        {
            Debug.LogWarning($"{gameObject.name}: Checkpoint {currentCheckpoint} is null, skipping.");
            currentCheckpoint++;
        }

        if (currentCheckpoint >= path.checkpoints.Count)
        {
            pathComplete = true;
            Debug.Log($"{gameObject.name}: Reached the end of the path.");
            return;
        }

        var checkpoint = path.checkpoints[currentCheckpoint];

        // Move towards checkpoint
        Vector3 target = checkpoint.transform.position;
        float speed = (entity != null && entity.enemyDataSO != null) ? entity.enemyDataSO.moveSpeed : 3f;

        float distance = Vector3.Distance(transform.position, target);
        // Debug.Log($"{gameObject.name}: Moving towards checkpoint {currentCheckpoint} at {target}, distance: {distance:F3}");

        // Only move if not already at the checkpoint
        if (distance > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            // Debug.Log($"{gameObject.name}: Arrived at checkpoint {currentCheckpoint}, waiting ({waitTimer:F2}/{checkpoint.waitTime:F2})");
            // At checkpoint, start waiting
            waitTimer += Time.deltaTime;
            if (waitTimer >= checkpoint.waitTime)
            {
                // Debug.Log($"{gameObject.name}: Finished waiting at checkpoint {currentCheckpoint}, moving to next.");
                currentCheckpoint++;
                waitTimer = 0f;
            }
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
            unit.AddBlockedEnemy(this);
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
            unit.RemoveBlockedEnemy(this);
            blockingUnit = null;
        }
    }

    // Example: Call this method when the enemy reaches the base
    private void OnReachBase()
    {
        // Notify GameManager that an enemy has reached the base (count as slain)
        GameManager.Instance?.IncrementEnemySlainCounter();

        // ...existing logic for damaging player/base, etc...
        Destroy(gameObject);
    }
}