using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSelectionUI : MonoBehaviour
{
    [Tooltip("Assign the retreat button from the UI prefab")]
    [SerializeField]
    private Button retreatButton;

    private Unit targetUnit;

    private void Awake()
    {
        if (retreatButton == null)
        {
            Debug.LogError("Failed to reference retreatButton");
        }
        else
        {
            retreatButton.onClick.AddListener(OnRetreatClicked);
        }
    }

    public void Initialize(Unit unit)
    {
        targetUnit = unit;
        if(targetUnit == null)
        {
            PlacementUIManager.Instance.HideUIDirection();
        }
    }

    private void OnRetreatClicked()
    {
        Debug.Log("Retreat button clicked!");
        if(targetUnit != null)
        {
            //Phase 2
            targetUnit.InitiateRetreat();
            gameObject.SetActive(false);
        }
        else
        {
            PlacementUIManager.Instance?.HideUIDirection();
        }
    }

    // --- Phase 3 Placeholder ---
    // Add methods here later for handling the "second drag" (direction setting)
    // These methods will calculate rotation and call targetUnit.ConfirmPlacement(rotation)
    // and PlacementUIManager.Instance.HideDirectionUI()

    private void OnDestroy()
    {
        if(retreatButton != null)
        {
            retreatButton.onClick.RemoveListener(OnRetreatClicked);
        }
    }
}
