// DeploymentManager.cs (Add this method to the existing Singleton script)

using System.Collections.Generic;
using UnityEngine;

public class DeploymentManager : MonoBehaviour
{
    public static DeploymentManager Instance { get; private set; }
    private Dictionary<int, int> deployedCounts = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional
        }
    }

    // Keep CanDeploy, RegisterDeployment, UnregisterDeployment...

    public bool CanDeploy(GameObject unitPrefab, int maxDeployCount)
    {
        if (unitPrefab == null) return false;
        int key = unitPrefab.GetInstanceID();
        int currentCount = deployedCounts.ContainsKey(key) ? deployedCounts[key] : 0;
        bool canDeploy = currentCount < maxDeployCount;
        return canDeploy;
    }

    public void RegisterDeployment(GameObject unitPrefab)
    {
        if (unitPrefab == null) return;
        int key = unitPrefab.GetInstanceID();
        if (deployedCounts.ContainsKey(key)) deployedCounts[key]++;
        else deployedCounts[key] = 1;
    }

    public void UnregisterDeployment(GameObject unitPrefab)
    {
        if (unitPrefab == null) return;
        int key = unitPrefab.GetInstanceID();
        if (deployedCounts.ContainsKey(key))
        {
            if (deployedCounts[key] > 0)
            {
                deployedCounts[key]--;
            }
        }
        else Debug.LogWarning($"Attempted to unregister {unitPrefab.name}, but it wasn't tracked.");
    }

    public int GetCurrentDeploymentCount(GameObject unitPrefab)
    {
        if (unitPrefab == null) return 0;
        int key = unitPrefab.GetInstanceID();
        return deployedCounts.ContainsKey(key) ? deployedCounts[key] : 0;
        
    }
}