// DirectionControlUI.cs (Script on Panel Root, Controls Child Handle)
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))] // Panel should have one
[RequireComponent(typeof(Canvas))]       // Panel should have the World Space Canvas
[RequireComponent(typeof(GraphicRaycaster))]
public class DirectionControlUI : MonoBehaviour
{
    [Header("Child UI References")]
    [Tooltip("Assign the child 'Retreat' Button GameObject")]
    [SerializeField] private Button retreatButton;
    [Tooltip("Assign the child 'DirectionHandle' Image GameObject")]
    [SerializeField] private Image directionHandleImage; // The draggable visual handle

    [Header("Drag Settings")]
    [SerializeField] private float dragRadius = 50f;
    [SerializeField] private float deadZoneRadius = 10f;
    [SerializeField] private Vector2 defaultDirection = Vector2.up;
    [SerializeField] private float defaultAngleDegrees = 90f; // Default to Up


    // --- State & References ---
    private Unit targetUnit;
    private RectTransform panelRectTransform; // This panel's RectTransform (the parent of the handle)
    private RectTransform handleRectTransform; // The handle's RectTransform (the child we move)
    private Camera eventCamera;
    private bool isDraggingHandle = false;
    private Vector2 handleInitialLocalPos; // Initial position of handle within panel (likely 0,0)

    void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>(); // Get self

        // Get handle's RectTransform from the assigned Image
        if (directionHandleImage != null)
        {
            handleRectTransform = directionHandleImage.GetComponent<RectTransform>();
            if (handleRectTransform != null)
            {
                handleInitialLocalPos = handleRectTransform.anchoredPosition; // Store handle's start pos (e.g., 0,0)
            }
            else { Debug.LogError("Direction Handle Image needs a RectTransform!", this); }
        }
        else { Debug.LogError("Direction Handle Image not assigned in Inspector!", this); }

        // Setup Retreat Button
        if (retreatButton != null) { retreatButton.onClick.AddListener(OnRetreatClicked); }
        else { Debug.LogError("Retreat Button not assigned!", this); }
    }

    public void Initialize(Unit unit)
    {
        targetUnit = unit;
        Canvas localCanvas = GetComponent<Canvas>();
        if (localCanvas != null) { eventCamera = localCanvas.worldCamera; }
        else { Debug.LogError("DirectionControlUI requires a Canvas component on the same GameObject!", this); }

        if (targetUnit == null) { Destroy(gameObject); return; }

        // Reset handle visuals on init
        ResetHandleVisuals();
    }

    // Reset handle to center/default rotation
    private void ResetHandleVisuals()
    {
        if (handleRectTransform == null) return;
        handleRectTransform.anchoredPosition = handleInitialLocalPos;
        float initialAngle = Mathf.Atan2(defaultDirection.x, defaultDirection.y) * Mathf.Rad2Deg;
        handleRectTransform.localRotation = Quaternion.Euler(0, 0, initialAngle);
    }

    // --- Methods Called by Event Trigger on DirectionHandle CHILD ---

    public void HandleBeginDrag(BaseEventData baseData)
    {
        PointerEventData eventData = baseData as PointerEventData;
        if (targetUnit == null) return;
        if (!RectTransformUtility.RectangleContainsScreenPoint(handleRectTransform, eventData.position, eventData.pressEventCamera ?? eventCamera)) { eventData.pointerDrag = null; return; } // Ensure drag starts on handle
        isDraggingHandle = true;
        InputManager.SignalUIDragActive();
    }

    public void HandleDrag(BaseEventData baseData)
    {
        if (!isDraggingHandle || targetUnit == null || handleRectTransform == null || panelRectTransform == null) return;
        PointerEventData eventData = baseData as PointerEventData;
        // Calculate pointer position relative to the PARENT PANEL's pivot
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera ?? eventCamera, out localPoint);

        float distance = localPoint.magnitude;
        Vector2 direction = (distance > 0.01f) ? localPoint.normalized : (Quaternion.Euler(0, 0, handleRectTransform.localEulerAngles.z) * Vector2.up); // Use current visual dir if no move

        // Clamp Handle Position
        float clampedDistance = Mathf.Min(distance, dragRadius);
        Vector2 targetHandlePosition = direction * clampedDistance;
        handleRectTransform.anchoredPosition = targetHandlePosition;

        // Update Handle Rotation (Angle relative to positive X-axis for Atan2(y,x))
        float visualAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        handleRectTransform.localRotation = Quaternion.Euler(0, 0, visualAngle - 90f); // Offset if handle sprite faces UP
    }

    public void HandleEndDrag(BaseEventData baseData)
    {
        if (!isDraggingHandle || targetUnit == null || handleRectTransform == null || panelRectTransform == null) return;
        isDraggingHandle = false;

        // Use the handle's FINAL Anchored Position relative to the panel center
        Vector2 finalHandlePosition = handleRectTransform.anchoredPosition;
        float finalDistance = finalHandlePosition.magnitude;

        float finalAngleDegrees; // The angle we will apply to the Unit's Z rotation

        // Check Deadzone
        if (finalDistance < deadZoneRadius)
        {
            Debug.Log("Drag ended inside deadzone, using default angle.");
            finalAngleDegrees = defaultAngleDegrees; // Use the defined default angle
            ResetHandleVisuals(); // Reset handle visuals
        }
        else
        {
            // Calculate direction vector FROM the final handle position
            Vector2 finalDirection = finalHandlePosition.normalized;
            // Calculate angle relative to the positive X-axis (Right)
            finalAngleDegrees = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;
            // No need to reset handle visuals here, let it stay where released
        }

        // Create the final rotation for the UNIT using the calculated angle
        Quaternion finalUnitRotation = Quaternion.Euler(0, 0, finalAngleDegrees);

        Debug.Log($"Direction Drag Ended. Final Handle Position: {finalHandlePosition}, Calculated Angle (X-axis): {finalAngleDegrees}, Final Unit Rotation: {finalUnitRotation.eulerAngles}");

        // Confirm Placement (Unit handles state change & destroying this UI)
        targetUnit.ConfirmPlacement(finalUnitRotation);

        // Register Deployment
        if (targetUnit.SourcePrefab != null) { DeploymentManager.Instance?.RegisterDeployment(targetUnit.SourcePrefab); }
        else { /* Log Error */ }

        // Signal Drag End
        InputManager.SignalUIDragInactive();
        // This GameObject is destroyed by targetUnit.ConfirmPlacement()
    }

    // Keep OnRetreatClicked, Initialize, Awake, OnDestroy...
    private void OnRetreatClicked() { /* ... Same ... */ if (targetUnit != null) targetUnit.InitiateRetreat(); else Destroy(gameObject); }
    void OnDestroy() { /* ... Same ... */ if (retreatButton != null) retreatButton.onClick.RemoveListener(OnRetreatClicked); PlacementUIManager.Instance?.NotifyDirectionUIHidden(); InputManager.SignalUIDragInactive(); }
}