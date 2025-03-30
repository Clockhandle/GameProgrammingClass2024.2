using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragToScreen : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private TileManager tileManager;

    private Vector2 startPosition;
    private Vector3Int closestTile;
    private bool canPlace;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (!canvas)
        {
            Debug.LogError("Canvas not found! Ensure the object is inside a Canvas.");
        }
    }

    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        tileManager.HighlightPlaceableTiles();
        canPlace = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPos = InputManager.MouseWorldPosition;
        closestTile = tileManager.GetClosestPlaceableTile(worldPos, out canPlace);

        if (canPlace)
        {
            SnapToTile(closestTile);
        }
        else
        {
            // Immediately snap the UI element to the mouse position in canvas space
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,      // Use the current pointer screen position
                canvas.worldCamera,      // Provide the correct camera (or null if using Overlay)
                out localPoint);
            rectTransform.anchoredPosition = localPoint;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        tileManager.tileHighlighter.ClearHighlights();

        if (canPlace)
        {
            Vector3 snapPosition = tileManager.tilemap.GetCellCenterWorld(closestTile);
            bool placed = tileManager.TryPlaceCharacter(snapPosition, characterPrefab);

            if (placed)
            {
                gameObject.SetActive(false);
            }
            else
            {
                ResetPosition();
            }
        }
        else
        {
            ResetPosition();
        }
    }

    private void SnapToTile(Vector3Int tilePosition)
    {
        Vector3 worldPos = tileManager.tilemap.GetCellCenterWorld(tilePosition);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, screenPos, canvas.worldCamera, out Vector2 anchoredPos))
        {
            rectTransform.anchoredPosition = anchoredPos;
        }
    }

    private void SnapToMouse(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = startPosition;
    }
}
