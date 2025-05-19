using UnityEngine;

public class EnemyPathFollower : MonoBehaviour
{
    private EnemyPath path;
    private int currentCheckpoint = 0;
    private float waitTimer = 0f;

    public void SetPath(EnemyPath newPath)
    {
        path = newPath;
        currentCheckpoint = 0;
        waitTimer = 0f;
    }

    void Update()
    {
        if (path == null || path.checkpoints.Count == 0) return;

        var checkpoint = path.checkpoints[currentCheckpoint];
        if (waitTimer < checkpoint.waitTime)
        {
            waitTimer += Time.deltaTime;
            return;
        }

        // Move towards checkpoint
        Vector3 target = checkpoint.transform.position;
        float speed = 3/* get from EnemyBase or EnemyDataSO */;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentCheckpoint++;
            waitTimer = 0f;
            if (currentCheckpoint >= path.checkpoints.Count)
            {
                // Reached end of path, handle as needed (destroy, loop, etc.)
            }
        }
    }
}
