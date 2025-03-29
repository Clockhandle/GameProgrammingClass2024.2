using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class DragToScreen : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;
    [SerializeField]
    private GameObject characterPrefab;
    [SerializeField]
    private TileManager tileManager;

    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("Canvas not found! Make sure the object is inside a Canvas.");
        }
        else
        {
            Debug.Log("Canvas found: " + canvas.name);
        }
    }


    private void Start()
    {
        startPosition = rectTransform.anchoredPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On Begin Drag!");
        tileManager.HighlightPlaceableTiles();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        tileManager.tileHighlighter.ClearHighlights();
        // Convert the drop position from screen space to world space.
        Vector3 dropWorldPos = InputManager.MouseWorldPosition;

        // Use the TileManager to attempt character placement.
        bool placed = tileManager.TryPlaceCharacter(dropWorldPos, characterPrefab);

        if (!placed)
        {
            Debug.Log("Invalid placement: tile is unplaceable or already occupied.");
        }

        // Reset the UI element's position and disable it.
        rectTransform.anchoredPosition = startPosition;
        gameObject.SetActive(false);
    }

    //private Vector3 GetWorldPositionFromUI(Vector2 screenPosition)
    //{
    //    float depth = -Camera.main.transform.position.z; // For 2D, if Camera is at z = -10, depth becomes 10.
    //    return Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, depth));
    //}
}
