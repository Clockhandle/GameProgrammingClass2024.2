using System.Collections.Generic;
using UnityEngine;

public class UnitRangeArcher : MonoBehaviour
{
    [Header("Setup")]
 private Transform arrowSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;

    private Unit unit;

    public void Initialize(Unit unit)
    {
        this.unit = unit;

       
         arrowSpawnPoint = this.transform;

        if (arrowPrefab == null && unit.ArrowPrefab != null)
            arrowPrefab = unit.ArrowPrefab;
    }

   
    public void FireArrowAnimationEvent()
    {
        if (!unit.IsOperational || unit.enemiesInRangeList.Count == 0)
            return;

        var target = unit.enemiesInRangeList[0];
        if (target == null || arrowPrefab == null || arrowSpawnPoint == null)
            return;

        GameObject arrowObj = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        if (arrow != null)
        {
            int damage = unit.unitDataSO != null ? unit.unitDataSO.attackDamage : 1;
            arrow.Initialize(target.transform, damage);
        }
    }
}
