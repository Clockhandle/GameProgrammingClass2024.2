using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementUIManager : MonoBehaviour
{
    public static PlacementUIManager Instance { get; set; }

    [SerializeField]
    private GameObject directionSelectionUIPrefab;
    [SerializeField]
    private Canvas parentCanvas;
    [SerializeField]
    private Vector2 UIOffset;

    private GameObject currentDirectionUIInstance = null;
    private Camera mainCamera;

    public bool IsDirectionUIShown
    {
        get
        {
            return currentDirectionUIInstance != null && currentDirectionUIInstance.activeInHierarchy;
        }
    }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        mainCamera = Camera.main;
        if(parentCanvas == null) parentCanvas = FindObjectOfType<Canvas>();
    }

    public void ShowDirectionUIForUnit(Unit targetUnit)
    {
        HideUIDirection();

        currentDirectionUIInstance = Instantiate(directionSelectionUIPrefab, parentCanvas.transform);
        currentDirectionUIInstance.name = $"DirectionUI_{targetUnit.name}";

        PositionAtUnit(currentDirectionUIInstance, targetUnit);

        DirectionSelectionUI uiScript = currentDirectionUIInstance.GetComponent<DirectionSelectionUI>();
        if(uiScript != null)
        {
            uiScript.Initialize(targetUnit);
        }
        else 
        {
            Debug.LogError("Direction UI Prefab is missing the DirectionSelectionUI script");
        }

        currentDirectionUIInstance.SetActive(true);
    }

    private void PositionAtUnit(GameObject uiInstance, Unit targetUnit)
    {
        Vector3 unitWorldPos = targetUnit.transform.position;
        Vector2 screenPos = mainCamera.WorldToScreenPoint(unitWorldPos);
        RectTransform uiRect = uiInstance.GetComponent<RectTransform>();
        if(uiRect != null)
        {
            Vector2 basePosition = ScreenToAnchoredPosition(screenPos, parentCanvas);
            uiRect.anchoredPosition = basePosition + UIOffset;
        }
    }
    public void HideUIDirection()
    {
        if(currentDirectionUIInstance != null)
        {
            Destroy(currentDirectionUIInstance);
            currentDirectionUIInstance = null;
        }
    }
    private Vector2 ScreenToAnchoredPosition(Vector2 screenPos, Canvas canvas)
    {
        Vector2 localPoint = Vector2.zero;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, canvas.worldCamera != null ? canvas.worldCamera : mainCamera, out localPoint);
        }
        return localPoint;
    }
}
