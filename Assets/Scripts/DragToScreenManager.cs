// DragToScreenManager.cs (Test Version with TEMPORARY Direct Placement)

using TowerDefense.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragToScreenManager : MonoBehaviour
{
    [Header("Required References")]
    [SerializeField] private TileManager tileManager;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private RectTransform dragGhostIconParent;
    [SerializeField] private GameObject dragGhostIconPrefab;

    // --- State ---
    private GameObject currentDraggedPrefab = null;
    private Unit currentUnitDataOnPrefab = null;
    private UnitIconData currentlyDraggedIconData = null; // Store reference to the original icon's data component
    private bool isDragging = false;
    private bool canPlace;
    private Vector3Int closestTile;

    // Drag Target State
    private GameObject ghostIconInstance = null;
    private RectTransform ghostIconRectTransform = null;
    private RectTransform draggedOriginalIconRect = null;
    private Transform originalParent = null;
    private int originalSiblingIndex;
    private Vector2 originalAnchoredPosition;

    // --- Dependencies ---
    private DeploymentManager deploymentManager;

    private void Awake()
    {
        deploymentManager = DeploymentManager.Instance; // Use Singleton

        // --- Validate References --- (Same as before)
        bool referencesValid = true;
        if (tileManager == null) { Debug.LogError("DragToScreenManager: TileManager not assigned!", this); referencesValid = false; }
        if (parentCanvas == null) parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null) { Debug.LogError("DragToScreenManager: Parent Canvas not found!", this); referencesValid = false; }
        if (deploymentManager == null) { Debug.LogError("DragToScreenManager: DeploymentManager Singleton Instance not found!", this); referencesValid = false; }
        if (dragGhostIconPrefab == null) { Debug.LogError("DragToScreenManager: Drag Ghost Icon Prefab not assigned!", this); referencesValid = false; }
        if (dragGhostIconParent == null) dragGhostIconParent = parentCanvas.transform as RectTransform;

        if (!referencesValid) { Debug.LogError("DragToScreenManager disabled due to missing references."); this.enabled = false; }
    }

    // --- Event Handlers ---

    public void HandleBeginDrag(BaseEventData baseData)
    {
        if (!this.enabled) return;
        PointerEventData eventData = baseData as PointerEventData;
        if (eventData == null || eventData.pointerDrag == null) return;

        // Store reference to the icon data component being dragged
        currentlyDraggedIconData = eventData.pointerDrag.GetComponent<UnitIconData>();
        if (currentlyDraggedIconData == null || currentlyDraggedIconData.unitPrefab == null) { CancelDrag(); return; }

        Unit unitComp = currentlyDraggedIconData.unitPrefab.GetComponent<Unit>();
        if (unitComp == null || unitComp.unitDataSO == null) { CancelDrag(); return; }

        // --- Check Deployment Limit ---
        int maxCount = unitComp.unitDataSO.maxNumberOfDeployments;
        if (!deploymentManager.CanDeploy(currentlyDraggedIconData.unitPrefab, maxCount))
        {
            Debug.Log($"Deployment limit reached for {currentlyDraggedIconData.unitPrefab.name}. Drag cancelled.");
            CancelDrag(); return;
        }

        // --- Decide Drag Type based on Count ---
        int currentCount = deploymentManager.GetCurrentDeploymentCount(currentlyDraggedIconData.unitPrefab);
        bool isLastOne = (maxCount - currentCount <= 1);

        isDragging = true;
        currentDraggedPrefab = currentlyDraggedIconData.unitPrefab;
        currentUnitDataOnPrefab = unitComp;
        InputManager.SignalUIDragStart();

        if (isLastOne)
        {
            // --- Drag Original Icon ---
            ghostIconInstance = null; ghostIconRectTransform = null; // Ensure no ghost
            draggedOriginalIconRect = currentlyDraggedIconData.GetComponent<RectTransform>();
            if (draggedOriginalIconRect == null) { CancelDrag(); return; }
            originalParent = draggedOriginalIconRect.parent;
            originalSiblingIndex = draggedOriginalIconRect.GetSiblingIndex();
            originalAnchoredPosition = draggedOriginalIconRect.anchoredPosition;
            draggedOriginalIconRect.SetParent(dragGhostIconParent, true);
            draggedOriginalIconRect.SetAsLastSibling();
            ToggleRaycasts(draggedOriginalIconRect, false); // Disable raycasts on original while dragging
            UpdateOriginalIconPosition(eventData);
        }
        else
        {
            // --- Drag Ghost Icon ---
            draggedOriginalIconRect = null; originalParent = null; // Ensure no original tracked
            CreateGhostIcon(currentlyDraggedIconData, eventData);
            // Optional: Visually disable original icon
            // currentlyDraggedIconData.gameObject.SetActive(false);
        }

        // --- Common Drag Start Actions ---
        if (tileManager != null) tileManager.HighlightPlaceableTiles(currentDraggedPrefab);
        canPlace = false;
    }


    public void HandleDrag(BaseEventData baseData)
    {
        if (!isDragging) return;
        PointerEventData eventData = baseData as PointerEventData;
        if (eventData == null || tileManager == null || currentDraggedPrefab == null) return;

        Vector3 worldPos = InputManager.MouseWorldPosition;
        closestTile = tileManager.GetClosestPlaceableTile(worldPos, currentDraggedPrefab, out canPlace);

        // --- Update Position based on Drag Type ---
        if (ghostIconInstance != null)
        {
            UpdateGhostIconPosition(eventData);
            if (canPlace) SnapGhostIconToTile(closestTile);
        }
        else if (draggedOriginalIconRect != null)
        {
            UpdateOriginalIconPosition(eventData);
            if (canPlace) SnapOriginalIconToTile(closestTile);
        }
    }


    public void HandleEndDrag(BaseEventData baseData)
    {
        if (!isDragging) return;

        InputManager.SignalUIDragEnd();

        if (tileManager?.tileHighlighter != null)
            tileManager.tileHighlighter.ClearHighlights(); // Or ClearAllTrackedHighlights

        // --- ================================================ ---
        // --- TEMPORARY - TESTING ORIGINAL PLACEMENT LOGIC ---
        // --- ================================================ ---
        if (canPlace)
        {
            Vector3 snapPosition = tileManager.tilemap.GetCellCenterWorld(closestTile);

            // Check if deployment is allowed for this unit prefab.
            if (deploymentManager.CanDeploy(currentDraggedPrefab, currentUnitDataOnPrefab.unitDataSO.maxNumberOfDeployments))
            {
                // --- Call OLD placement method ---
                // Make sure TileManager HAS this method for testing
                bool placed = tileManager.TryPlaceCharacter(snapPosition, currentDraggedPrefab);

                if (placed)
                {
                    // Register the deployment IMMEDIATELY
                    deploymentManager.RegisterDeployment(currentDraggedPrefab);
                    Debug.Log($"TEMP: Placed and registered {currentDraggedPrefab.name}");

                    // After registering, check if we've reached the limit for the ORIGINAL icon
                    if (!deploymentManager.CanDeploy(currentDraggedPrefab, currentUnitDataOnPrefab.unitDataSO.maxNumberOfDeployments))
                    {
                        // Limit reached: disable the ORIGINAL UI element.
                        Debug.Log($"TEMP: Limit reached for {currentDraggedPrefab.name}, disabling original icon.");
                        if (currentlyDraggedIconData != null)
                        {
                            currentlyDraggedIconData.gameObject.SetActive(false);
                        }
                    }
                    // If limit not reached, the original icon will be reset by cleanup logic below.
                }
                else
                {
                    Debug.Log("TEMP: Placement failed on final TileManager check.");
                    // Original icon will be reset by cleanup logic below.
                }
            }
            else
            {
                // Limit was already reached before this placement attempt
                Debug.Log("TEMP: Deployment limit reached before final placement check.");
                if (currentlyDraggedIconData != null)
                {
                    // Disable the original icon if limit was met
                    // currentlyDraggedIconData.gameObject.SetActive(false); // Decide if you want this behaviour
                }
            }
        }
        else
        {
            Debug.Log("TEMP: Drag ended over non-placeable area.");
            // Original icon will be reset by cleanup logic below.
        }
        // --- ================================================ ---
        // --- END TEMPORARY TEST LOGIC                     ---
        // --- ================================================ ---


        // --- Cleanup ---
        // This part remains the same, handles resetting original icon or destroying ghost
        if (ghostIconInstance != null)
        {
            DestroyGhostIcon();
            // Optional: Re-enable original icon if it was disabled during ghost drag
            // if(currentlyDraggedIconData != null) currentlyDraggedIconData.gameObject.SetActive(true);
        }
        else if (draggedOriginalIconRect != null)
        {
            // Only reset the original icon if it hasn't been disabled above by reaching deploy limit
            if (currentlyDraggedIconData != null && currentlyDraggedIconData.gameObject.activeSelf)
            {
                ResetOriginalIcon();
            }
            else if (draggedOriginalIconRect != null) // Ensure ref is cleared if icon was disabled
            {
                // Icon was disabled, just clear the manager's reference to it
                draggedOriginalIconRect = null;
                originalParent = null;
            }
        }

        ResetDragState(); // Reset internal manager state
    }

    // --- Helper Methods --- (Keep all helpers: CreateGhostIcon, DestroyGhostIcon, ResetOriginalIcon, UpdateGhostIconPosition, UpdateOriginalIconPosition, SnapGhostIconToTile, SnapOriginalIconToTile, ResetDragState, CancelDrag, ToggleRaycasts)
    // ... (Include all helper methods from the previous response here) ...
    private void CreateGhostIcon(UnitIconData iconData, PointerEventData eventData)
    {
        if (dragGhostIconPrefab == null) return;
        ghostIconInstance = Instantiate(dragGhostIconPrefab, dragGhostIconParent);
        ghostIconInstance.name = $"GhostIcon_{iconData.unitPrefab.name}";
        Image ghostImage = ghostIconInstance.GetComponent<Image>();
        Image originalImage = iconData.GetComponent<Image>();
        if (ghostImage != null && originalImage != null && originalImage.sprite != null)
        {
            ghostImage.sprite = originalImage.sprite; ghostImage.color = originalImage.color;
            ghostImage.raycastTarget = false;
        }
        else if (ghostImage != null) ghostImage.raycastTarget = false;
        ghostIconRectTransform = ghostIconInstance.GetComponent<RectTransform>();
        if (ghostIconRectTransform != null) UpdateGhostIconPosition(eventData);
        else Debug.LogError("Ghost Icon Prefab missing RectTransform!", ghostIconInstance);
        ghostIconInstance.SetActive(true);
    }
    private void DestroyGhostIcon() 
    { 
        if (ghostIconInstance != null) 
        { 
            Destroy(ghostIconInstance);
            ghostIconInstance = null; 
            ghostIconRectTransform = null;
        } 
    }
    private void ResetOriginalIcon()
    {
        if (draggedOriginalIconRect == null) return;
        if (originalParent != null)
        {
            draggedOriginalIconRect.SetParent(originalParent, true);
            draggedOriginalIconRect.SetSiblingIndex(originalSiblingIndex);
        }
        draggedOriginalIconRect.anchoredPosition = originalAnchoredPosition;
        ToggleRaycasts(draggedOriginalIconRect, true);
        draggedOriginalIconRect = null; originalParent = null;
    }
    private void UpdateGhostIconPosition(PointerEventData eventData) 
    { 
        if (ghostIconRectTransform == null || parentCanvas == null) return; 
        Vector2 localPoint; 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out localPoint); 
        ghostIconRectTransform.anchoredPosition = localPoint; 
    }
    private void UpdateOriginalIconPosition(PointerEventData eventData) 
    { 
        if (draggedOriginalIconRect == null || parentCanvas == null) return;
        Vector2 localPoint; 
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, eventData.position, parentCanvas.worldCamera, out localPoint);
        draggedOriginalIconRect.anchoredPosition = localPoint; 
    }
    private void SnapGhostIconToTile(Vector3Int tilePosition) 
    { 
        if (tileManager == null || parentCanvas == null || ghostIconRectTransform == null || Camera.main == null) return;
        Vector3 worldPos = tileManager.tilemap.GetCellCenterWorld(tilePosition); Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 anchoredPos; 
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out anchoredPos)) 
        { 
            ghostIconRectTransform.anchoredPosition = anchoredPos; 
        } 
    }
    private void SnapOriginalIconToTile(Vector3Int tilePosition)
    { 
        if (tileManager == null || parentCanvas == null || draggedOriginalIconRect == null || Camera.main == null) return; 
        Vector3 worldPos = tileManager.tilemap.GetCellCenterWorld(tilePosition); Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos); 
        Vector2 anchoredPos; if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out anchoredPos)) 
        { 
            draggedOriginalIconRect.anchoredPosition = anchoredPos; 
        } 
    }
    private void ResetDragState() 
    { 
        isDragging = false;
        currentDraggedPrefab = null;
        currentUnitDataOnPrefab = null;
        canPlace = false;
        ghostIconInstance = null;
        ghostIconRectTransform = null;
        draggedOriginalIconRect = null;
        originalParent = null;
        currentlyDraggedIconData = null;
    } // Added currentlyDraggedIconData = null
    private void CancelDrag() 
    { 
        Debug.Log("Drag cancelled during initiation."); 
        DestroyGhostIcon(); 
        if (draggedOriginalIconRect != null) ResetOriginalIcon(); 
        ResetDragState(); 
    }
    private void ToggleRaycasts(Transform target, bool enable) 
    { 
        Graphic g = target.GetComponent<Graphic>(); 
        if (g != null) g.raycastTarget = enable; 
        foreach (Transform child in target) 
        {
            ToggleRaycasts(child, enable);
        } 
    }

}