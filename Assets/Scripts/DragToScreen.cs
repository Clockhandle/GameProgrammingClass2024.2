using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class DragToScreen : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform _rectTransform;
    private Canvas _canvas;
    [SerializeField]
    private GameObject _prefabToSpawn;
    private Vector2 _startPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        _startPosition = _rectTransform.anchoredPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("On Begin Drag!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("On Drag!");
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("On End Drag!");
        Vector3 worldPosition = GetWorldPositionFromUI(eventData.position);

        Instantiate(_prefabToSpawn, worldPosition, Quaternion.identity);

        _rectTransform.anchoredPosition = _startPosition;
        gameObject.SetActive(false);
    }

    private Vector3 GetWorldPositionFromUI(Vector2 position)
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);
        if (hit)
        {
            Debug.Log("Hit Object: " + hit.gameObject.name);
        }
        return worldPosition; // Ensure a Vector3 is returned
    }
}
