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
    private Unit unit;
    private DeploymentManager deploymentManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        unit = characterPrefab.GetComponent<Unit>();
        deploymentManager = new DeploymentManager();

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

        tileManager.GetAllowedTileFlags(unit);
        tileManager.HighlightPlaceableTiles(characterPrefab);
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

            // Check if deployment is allowed for this unit prefab.
            if (deploymentManager.CanDeploy(characterPrefab, unit.unitDataSO.maxNumberOfDeployments))
            {
                bool placed = tileManager.TryPlaceCharacter(snapPosition, characterPrefab);
                if (placed)
                {
                    // Register the deployment.
                    deploymentManager.RegisterDeployment(characterPrefab);

                    // After registering, check if we've reached the limit.
                    if (!deploymentManager.CanDeploy(characterPrefab, unit.unitDataSO.maxNumberOfDeployments))
                    {
                        // Limit reached: disable the UI element.
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        // Not yet reached limit: reset UI for another placement.
                        ResetPosition();
                    }
                }
                else
                {
                    Debug.Log("Placement failed on final check.");
                    ResetPosition();
                }
            }
            else
            {
                Debug.Log("Deployment limit reached for this unit type.");
                gameObject.SetActive(false);
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

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = startPosition;
    }

}
