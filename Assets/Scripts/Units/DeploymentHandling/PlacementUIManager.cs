// PlacementUIManager.cs (Simplified - State Tracker Role)
using UnityEngine;

public class PlacementUIManager : MonoBehaviour
{
    // --- Singleton Instance ---
    public static PlacementUIManager Instance { get; private set; }

    // --- State ---
    // Tracks if ANY direction selection UI is currently active
    private bool isDirectionUIShown = false;
    /// <summary>
    /// Returns true if the Direction Selection UI is currently active for any unit.
    /// Checked by CameraController, DragToScreenManager to block other inputs.
    /// </summary>
    public bool IsDirectionUIShown => isDirectionUIShown;

    private void Awake()
    {
        // Singleton Setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // Optional
    }

    /// <summary>
    /// Called by Unit script when its DirectionSelectionUI is shown.
    /// </summary>
    public void NotifyDirectionUIShown()
    {
        isDirectionUIShown = true;
    }

    /// <summary>
    /// Called by Unit script OR DirectionSelectionUI script when the UI is hidden/destroyed.
    /// </summary>
    public void NotifyDirectionUIHidden()
    {
        isDirectionUIShown = false;
    }

    // REMOVED: No longer needs prefab refs, canvas refs, or methods like
    // ShowDirectionUIForUnit, PositionUIAtUnit, ScreenToAnchoredPosition, HideUIDirection (with Destroy)
}