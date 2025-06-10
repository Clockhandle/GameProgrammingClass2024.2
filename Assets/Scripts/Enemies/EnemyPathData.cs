using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathData : MonoBehaviour
{
    [Tooltip("Ordered list of checkpoints for this path")]
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
}
