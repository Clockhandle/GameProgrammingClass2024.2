// UnitIconData.cs (Updated)
using UnityEngine;

public class UnitIconData : MonoBehaviour
{
    [Tooltip("The Unit Prefab this UI icon represents.")]
    public GameObject unitPrefab;

    // --- Store original state ---
    // Use public properties with private setters or [HideInInspector] public fields
    public Transform originalParent { get; private set; }
    public int originalSiblingIndex { get; private set; }
    public Vector2 originalAnchoredPosition { get; private set; }

    private bool initialStateStored = false;
    private RectTransform rectTransform; // Cache RectTransform

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        StoreInitialState(); // Store state when the icon first wakes up
    }

    // Call this if state needs to be re-captured (e.g., if list order changes dynamically)
    // Might not be needed if the list is static.
    public void StoreInitialState()
    {
        // Store only once, or if parent seems invalid (e.g., after being reparented during a failed drag reset)
        if (!initialStateStored || transform.parent == null || transform.parent == DragToScreenManager.Instance?.dragGhostIconParent) // Heuristic check
        {
            if (rectTransform != null && transform.parent != null) // Need parent to store state
            {
                originalParent = transform.parent;
                originalSiblingIndex = transform.GetSiblingIndex();
                originalAnchoredPosition = rectTransform.anchoredPosition;
                initialStateStored = true;

            }
            // else { Debug.LogWarning($"Could not store initial state for {gameObject.name}", this); }
        }
    }

    // Optional: Call this right before a drag starts on this specific icon
    // if you suspect state needs refreshing after potential external changes.
    public void EnsureInitialStateStored()
    {
        StoreInitialState();
    }
}