using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RedeploymentDrawerUI : MonoBehaviour
{
    public static RedeploymentDrawerUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform redeployListParent; // e.g., a HorizontalLayoutGroup
    [SerializeField] private GameObject redeployIconPrefab;


    // The img handle wait sprite 
   [SerializeField] private List<Image> redeploySlots;


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

    private void Awake()
    {
        Instance = this;
    }


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
        //// Clear old icons
        //foreach (var go in iconMap.Values)
        //    Destroy(go);
        //iconMap.Clear();

        //// Add new icons
        //foreach (var entry in RedeploymentManager.Instance.WaitList)
        //{
        //    Debug.Log("initiateRetreat called on an operational unit, adding to wait list.");

        //    var iconGO = Instantiate(iconPrefab, iconParent);
        //    // Set icon sprite
        //    iconGO.GetComponentInChildren<Image>().sprite = entry.unitIcon;
        //    iconMap[entry] = iconGO;
        //}

        // Clear all slots 
        foreach (var img in redeploySlots)
        {
            if (img != null)
            {
                img.sprite = null;
                img.color = new Color(1, 1, 1, 0); // Transparent
                img.gameObject.SetActive(false);  // Hide the slot
            }
        }

       
        int i = 0;
        foreach (var entry in RedeploymentManager.Instance.WaitList)
        {
            if (i >= redeploySlots.Count) break;

            var img = redeploySlots[i];

            img.sprite = entry.unitIcon;
            img.color = Color.white;             
            img.gameObject.SetActive(true);          
            i++;
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
