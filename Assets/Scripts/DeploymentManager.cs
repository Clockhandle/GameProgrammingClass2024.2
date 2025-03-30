using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeploymentManager
{
    // Maps each unit prefab to the number of instances deployed.
    private Dictionary<GameObject, int> deployedCounts = new Dictionary<GameObject, int>();

    public bool CanDeploy(GameObject unitPrefab, int maxDeployCount)
    {
        int currentCount = deployedCounts.ContainsKey(unitPrefab) ? deployedCounts[unitPrefab] : 0;
        return currentCount < maxDeployCount;
    }

    public void RegisterDeployment(GameObject unitPrefab)
    {
        if (deployedCounts.ContainsKey(unitPrefab))
            deployedCounts[unitPrefab]++;
        else
            deployedCounts[unitPrefab] = 1;
    }
}
