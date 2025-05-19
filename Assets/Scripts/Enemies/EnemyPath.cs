using UnityEngine;
using System.Collections.Generic;

public class EnemyPath : MonoBehaviour
{
    [Tooltip("Ordered list of checkpoints for this path")]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
}
