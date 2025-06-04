using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float idleTime =0;
    public float longRange = 4f;
    public float walkDurrationRanged = 2f;

    public float stuntTime = 1.5f;
    // Add more fields as needed (e.g., reward, sprite, etc.)
}