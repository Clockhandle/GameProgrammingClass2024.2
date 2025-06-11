using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum UnitType
{
    None = 0,
    Ground = 1 << 0,
    Ranged = 1 << 1
}

[System.Flags]
public enum UnitClass
{
    None = 0,
    Infantry = 1 << 0,
    Archer = 1 << 1,
    Medic = 1 << 2,
    Workers = 1 << 3,
    Scout = 1 << 4,
}
[CreateAssetMenu(fileName = "UnitData", menuName = "Data/Unit Data")]
public class UnitDataSO : ScriptableObject
{
 

    [Header("Stats")]
    public int maxHealth = 100;
    public int defense = 10;
    public float resistance = 0;
    public int SP = 10;
    public int blockCount = 1;
    public int attackDamage = 1;
    public float attackInterval = 1.0f;
    public float attackRange = 1;
    public Vector3 attackOffset;

    [Header("Healing")]
    public int healAmount = 2;

    [Header("Trait")]
    public UnitType type;
    public UnitClass unitClass;

    [Header("Deployment")]
    public int DP = 10;
    public int maxNumberOfDeployments = 5;
    public Sprite icon;
    public float redeployCooldown = 10f;

    [Header("Skill Cool Down")]
    public float skillCoolDown = 8f;

}
