using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("How long the enemy waits at this checkpoint (seconds)")]
    public float waitTime = 0f;

    public enum Direction { Up, Down, Left, Right }
    public Direction facingDirectionPoint;
}