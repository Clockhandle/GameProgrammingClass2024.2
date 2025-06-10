using UnityEngine;
using System.Collections.Generic;

public class EnemyPath : MonoBehaviour
{
    public List<Checkpoint> checkpoints = new();
    private EnemyDirectionHandler directionHandler;

    public void Initialize(List<Checkpoint> originalCheckpoints, EnemyDirectionHandler handler)
    {
        checkpoints = new List<Checkpoint>(originalCheckpoints);
        directionHandler = handler;

        if (checkpoints.Count > 0)
            ApplyCheckpointDirection(checkpoints[0]);
    }

    public void OnReachCheckpoint(int index)
    {
        if (index < 0 || index >= checkpoints.Count) return;
        ApplyCheckpointDirection(checkpoints[index]);
    }

    private void ApplyCheckpointDirection(Checkpoint checkpoint)
    {
        directionHandler.currentDirection = (EnemyDirectionHandler.FacingDirection)checkpoint.facingDirectionPoint;
        directionHandler.UpdateBasedOnAttackType();
    }

}
