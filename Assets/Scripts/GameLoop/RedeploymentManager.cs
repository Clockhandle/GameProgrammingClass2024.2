using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeploymentManager : MonoBehaviour
{
    public static RedeploymentManager Instance { get; private set; }

    // Represents a unit on cooldown
    public class RedeploymentEntry
    {
        public GameObject unitPrefab;
        public Sprite unitIcon;
        public float cooldownRemaining;
        public float cooldownTotal;
        public Action<RedeploymentEntry> onCooldownEnd; // Optional callback for UI

        public RedeploymentEntry(GameObject prefab, Sprite icon, float cooldown, Action<RedeploymentEntry> onEnd = null)
        {
            unitPrefab = prefab;
            unitIcon = icon;
            cooldownRemaining = cooldownTotal = cooldown;
            onCooldownEnd = onEnd;
        }
    }

    private readonly List<RedeploymentEntry> waitList = new List<RedeploymentEntry>();

    public IReadOnlyList<RedeploymentEntry> WaitList => waitList;

    public event Action OnWaitListChanged;

    // Event: invoked when a prefab's cooldown is finished
    public event Action<GameObject> OnRedeployReady;

    // Internal cooldown tracking
    private Dictionary<GameObject, float> cooldowns = new Dictionary<GameObject, float>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Update cooldowns
        for (int i = waitList.Count - 1; i >= 0; i--)
        {
            var entry = waitList[i];
            entry.cooldownRemaining -= Time.deltaTime;
            if (entry.cooldownRemaining <= 0f)
            {
                entry.onCooldownEnd?.Invoke(entry);
                waitList.RemoveAt(i);
                OnWaitListChanged?.Invoke();
            }
        }
    }

    /// <summary>
    /// Adds a unit to the redeployment wait list.
    /// </summary>
    public void AddToWaitList(GameObject unitPrefab, Sprite unitIcon, float cooldown, Action<RedeploymentEntry> onCooldownEnd = null)
    {
        if (unitPrefab == null) return;
        cooldowns[unitPrefab] = Time.time + cooldown;
        StartCoroutine(CooldownRoutine(unitPrefab, cooldown));

        var entry = new RedeploymentEntry(unitPrefab, unitIcon, cooldown, onCooldownEnd);
        waitList.Add(entry);
        OnWaitListChanged?.Invoke();
    }

    private IEnumerator CooldownRoutine(GameObject prefab, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        if (cooldowns.ContainsKey(prefab))
        {
            cooldowns.Remove(prefab);
            OnRedeployReady?.Invoke(prefab);
        }
    }

    /// <summary>
    /// Removes a unit from the wait list (if needed for manual removal).
    /// </summary>
    public void RemoveFromWaitList(RedeploymentEntry entry)
    {
        if (waitList.Remove(entry))
        {
            OnWaitListChanged?.Invoke();
        }
    }

    /// <summary>
    /// Returns true if the prefab is still on cooldown.
    /// </summary>
    public bool IsOnCooldown(GameObject prefab)
    {
        if (prefab == null) return false;
        return cooldowns.ContainsKey(prefab);
    }
}
