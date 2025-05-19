using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RedeploymentDrawerUI : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject drawerPanel;

    private Dictionary<RedeploymentManager.RedeploymentEntry, GameObject> iconMap = new();
    [SerializeField] private RectTransform drawerRect; // Assign your panel's RectTransform
    [SerializeField] private float openX = 0f;         // X position when open
    [SerializeField] private float closedX = -200f;    // X position when closed (adjust to your panel width)
    [SerializeField] private float slideSpeed = 800f;  // Pixels per second

    private bool isOpen = false;
    private float targetX;

    void Start()
    {
        // Start with drawer closed
        targetX = closedX;
        SetDrawerPosition(closedX);
        if (RedeploymentManager.Instance != null)
            RedeploymentManager.Instance.OnWaitListChanged += RefreshDrawer;
        else
            Debug.LogWarning("RedeploymentManager.Instance is null in Start");
    }

    void OnDisable()
    {
        if (RedeploymentManager.Instance != null)
            RedeploymentManager.Instance.OnWaitListChanged -= RefreshDrawer;
    }


    void Update()
    {
        // Animate the drawer position
        float currentX = drawerRect.anchoredPosition.x;
        if (Mathf.Abs(currentX - targetX) > 0.1f)
        {
            float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * 10f);
            SetDrawerPosition(newX);
        }

        // Update cooldown visuals (as before)
        foreach (var kvp in iconMap)
        {
            var entry = kvp.Key;
            var iconGO = kvp.Value;
            // Example: update fill amount if you add a fill image
            // iconGO.GetComponentInChildren<Image>().fillAmount = entry.cooldownRemaining / entry.cooldownTotal;
        }
    }

    void RefreshDrawer()
    {
        // Clear old icons
        foreach (var go in iconMap.Values)
            Destroy(go);
        iconMap.Clear();

        // Add new icons
        foreach (var entry in RedeploymentManager.Instance.WaitList)
        {
            Debug.Log("initiateRetreat called on an operational unit, adding to wait list.");

            var iconGO = Instantiate(iconPrefab, iconParent);
            // Set icon sprite
            iconGO.GetComponentInChildren<Image>().sprite = entry.unitIcon;
            iconMap[entry] = iconGO;
        }
    }

    public void ToggleDrawer()
    {
        isOpen = !isOpen;
        targetX = isOpen ? openX : closedX;
    }

    private void SetDrawerPosition(float x)
    {
        Vector2 pos = drawerRect.anchoredPosition;
        pos.x = x;
        drawerRect.anchoredPosition = pos;
    }
}
