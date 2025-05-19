// UnitSelectionManager.cs - improved to use existing UI
using UnityEngine;
using System.Collections.Generic;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }

    private Unit selectedUnit;
    private DirectionControlUI activeUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        InputManager.OnTap += HandleTap;
    }

    private void OnDisable()
    {
        InputManager.OnTap -= HandleTap;
    }

    private void HandleTap()
    {
        // Skip if UI is being interacted with
        if (InputManager.IsPointerOverUI || InputManager.IsSpecificUIDragging)
            return;

        // Raycast to find units
        Vector3 tapPosition = InputManager.MouseWorldPosition;
        RaycastHit2D[] hits = Physics2D.RaycastAll(tapPosition, Vector2.zero);

        Unit tappedUnit = null;
        foreach (var hit in hits)
        {
            tappedUnit = hit.collider.GetComponent<Unit>();
            if (tappedUnit == null)
                tappedUnit = hit.collider.GetComponentInParent<Unit>();

            if (tappedUnit != null)
                break;
        }

        // If we tapped on a deployed unit
        if (tappedUnit != null && tappedUnit.GetCurrentState() != null && !(tappedUnit.GetCurrentState() is UnitAwaitDeploymentState))
        {
            SelectUnit(tappedUnit);
        }
        else
        {
            ClearSelection();
        }
    }

    private void SelectUnit(Unit unit)
    {
        // Only clear if a different unit is selected
        if (selectedUnit != null && selectedUnit != unit)
        {
            ClearSelection();
        }

        selectedUnit = unit;

        // Show unit's range
        UnitRange range = selectedUnit.GetComponentInChildren<UnitRange>();
        if (range != null)
        {
            range.ShowRangePreview(true);
        }

        // Find existing DirectionControlUI component
        activeUI = selectedUnit.GetComponentInChildren<DirectionControlUI>(true);

        if (activeUI != null)
        {
            activeUI.gameObject.SetActive(true);
            activeUI.SetRetreatModeOnly(true);

            Canvas uiCanvas = activeUI.GetComponent<Canvas>();
            if (uiCanvas != null)
            {
                uiCanvas.worldCamera = Camera.main;
            }
        }
        else
        {
            Debug.LogWarning($"No DirectionControlUI found for unit {selectedUnit.name}. The prefab might be missing this component.");
        }
    }

    public void ClearSelection()
    {
        if (selectedUnit != null)
        {
            // Hide range
            UnitRange range = selectedUnit.GetComponentInChildren<UnitRange>();
            if (range != null)
            {
                range.ShowRangePreview(false);
            }

            // Hide UI rather than destroying it
            if (activeUI != null)
            {
                activeUI.gameObject.SetActive(false);
                activeUI = null;
            }

            selectedUnit = null;
        }
    }
}