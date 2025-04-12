// Unit.cs (Updated to Manage World Space UI)
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    public UnitDataSO unitDataSO;

    [Header("World Space UI")]
    [Tooltip("Assign the World Space Canvas Prefab for Direction Selection UI")]
    [SerializeField] private GameObject directionUIWorldPrefab;
    [SerializeField] private Vector3 uiLocalOffset = new Vector3(0, 1.5f, 0); // Offset from unit pivot

    private UnitStates currentStates;
    private GameObject directionUIInstance; // Holds the instantiated UI
    private GameObject sourcePrefab; // Added: Stores the original prefab

    // Optional: Cache camera if accessed frequently
    // private Camera mainCamera;

    void Awake()
    {
        // mainCamera = Camera.main; // Example caching
        // Ensure UnitDataSO is assigned
        if (unitDataSO == null) Debug.LogError("UnitDataSO not found on Unit!", this);
    }

    // Called immediately after instantiation by TileManager.TryPlaceCharacterProvisionally
    public void InitializeAwaitDeploymentState(/* Removed TileManager parameter */)
    {
        currentStates = new UnitAwaitDeploymentState();
        currentStates?.StartState(this);
        Debug.Log($"{gameObject.name} initialized in AwaitingDirectionState.");

        // --- Instantiate and Setup World Space UI ---
        if (directionUIWorldPrefab != null)
        {
            // Instantiate as a child of this Unit's transform
            directionUIInstance = Instantiate(directionUIWorldPrefab, transform);
            // Set local position relative to the unit pivot point
            directionUIInstance.transform.localPosition = uiLocalOffset;
            // Ensure default rotation relative to parent (can be adjusted later)
            directionUIInstance.transform.localRotation = Quaternion.identity;

            // Assign Main Camera to the World Space Canvas's Event Camera
            Canvas uiCanvas = directionUIInstance.GetComponent<Canvas>();
            if (uiCanvas != null && uiCanvas.renderMode == RenderMode.WorldSpace)
            {
                uiCanvas.worldCamera = Camera.main; // Use cached camera if available
            }
            else if (uiCanvas == null)
            {
                Debug.LogError("directionUIWorldPrefab is missing Canvas component!", directionUIInstance);
            }

            // Get the UI script and initialize it with this unit instance
            DirectionSelectionUI uiScript = directionUIInstance.GetComponent<DirectionSelectionUI>();
            if (uiScript != null)
            {
                uiScript.Initialize(this);
            }
            else
            {
                Debug.LogError("directionUIWorldPrefab is missing DirectionSelectionUI script component!", directionUIInstance);
            }

            // Notify manager that UI is active
            PlacementUIManager.Instance?.NotifyDirectionUIShown(); // Use null-conditional operator
        }
        else { Debug.LogWarning("directionUIWorldPrefab not assigned to Unit prefab! Cannot show direction UI.", this); }
        // --- End UI Setup ---

        // Optional: Disable unwanted components here
    }

    public void SetSourcePrefab(GameObject prefab)
    {
        this.sourcePrefab = prefab;
    }

    // Called externally (e.g., by DirectionSelectionUI) when direction is confirmed
    public void ConfirmPlacement(Quaternion finalRotation)
    {
        if (currentStates is UnitAwaitDeploymentState)
        {
            // --- Destroy Direction UI ---
            if (directionUIInstance != null) Destroy(directionUIInstance);
            PlacementUIManager.Instance?.NotifyDirectionUIHidden();
            // --- End Destroy UI ---

            transform.rotation = finalRotation; // Apply final rotation

            UnitStates operationalState = UnitStateFactory.CreateState(unitDataSO);
            SwitchState(operationalState); // Transition to Idle, ArcherState, etc.

            // Trigger deployment finalization actions
            PlayDeploymentAnimation();
            // EnableActiveComponents(true);

            Debug.Log($"{gameObject.name} placement confirmed facing {finalRotation.eulerAngles}. Switched to {operationalState?.GetType().Name}");
        }
        // ... (else block for wrong state) ...
    }

    // Called externally (e.g., by DirectionSelectionUI) when retreat is chosen
    public void InitiateRetreat()
    {
        // Store state before potentially destroying UI that calls this
        bool wasAwaitingDirection = currentStates is UnitAwaitDeploymentState;
        UnitStates stateBeforeRetreat = currentStates; // For logging if needed

        // Immediately destroy the UI if it exists (it's a child, will go away anyway, but cleaner)
        if (directionUIInstance != null)
        {
            Destroy(directionUIInstance);
        }

        // Notify UI manager state change (do this early)
        PlacementUIManager.Instance?.NotifyDirectionUIHidden();

        // Check if retreat is possible from the current state
        if (stateBeforeRetreat == null)
        {
            Debug.LogWarning($"InitiateRetreat called on {gameObject.name} but state was null.", this);
            // Destroy self just in case something went very wrong
            Destroy(gameObject);
            return;
        }

        // --- Logic for Retreating from Provisional State ---
        if (wasAwaitingDirection)
        {
            Debug.Log($"Retreat initiated for {gameObject.name} from AwaitingDeploymentState");

            // 1. Re-enable the original UI icon via DragToScreenManager
            if (sourcePrefab != null)
            {
                if (DragToScreenManager.Instance != null)
                {
                    DragToScreenManager.Instance.HandleUnitRetreat(sourcePrefab);
                }
                else { Debug.LogError($"DragToScreenManager Instance not found while trying to re-enable icon for {sourcePrefab.name}"); }
            }
            else { Debug.LogWarning($"Source prefab reference missing on {gameObject.name}, cannot re-enable UI icon."); }

            // 2. Unoccupy the Tile
            if (TileManager.Instance != null && TileManager.Instance.tileOccupancyCheck != null)
            {
                TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(transform.position, false);
            }
            else { Debug.LogError($"Cannot unoccupy tile for {gameObject.name}, TileManager missing!"); }

            // 3. Deployment count was never incremented, so no DeploymentManager notification needed.

            // 4. Destroy this Unit GameObject
            Destroy(gameObject);
        }
        // --- Logic for Retreating from Active State (Phase 2b/Later) ---
        // else if (currentStates is SomeActiveStateThatAllowsRetreat)
        // {
        //     Debug.Log($"Retreat initiated for {gameObject.name} from Active state {currentStates.GetType().Name}");
        //     // 1. Re-enable original UI Icon (maybe after cooldown?)
        //     // 2. Unoccupy Tile
        //     // 3. **** Call DeploymentManager.Instance.UnregisterDeployment(sourcePrefab); ****
        //     // 4. Trigger Cooldown logic...
        //     // 5. Destroy(gameObject);
        // }
        else
        {
            Debug.LogWarning($"InitiateRetreat called on {gameObject.name} from state {currentStates.GetType().Name} where retreat is not currently implemented.");
            // Decide if you should destroy anyway or do nothing. For now, let's destroy.
            if (TileManager.Instance != null && TileManager.Instance.tileOccupancyCheck != null) { TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(transform.position, false); }
            Destroy(gameObject); // Destroy even if state was unexpected? Or just log?
        }
    }

    private void Update()
    {
        currentStates?.UpdateState(this);
    }

    // Keep SwitchState and other helpers...
    public void SwitchState(UnitStates newState) { /* ... */ if (newState == null) return; currentStates?.ExitState(this); currentStates = newState; currentStates?.StartState(this); }
    private void PlayDeploymentAnimation() { Debug.Log($"Playing Deployment Animation for {gameObject.name}"); }

}