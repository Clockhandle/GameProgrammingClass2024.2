// Unit.cs (Updated to Manage World Space UI)
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    public UnitDataSO unitDataSO;

    [Header("World Space UI")]
    [Tooltip("Assign the World Space Canvas Prefab with DirectionControlUI script")]
    [SerializeField] private GameObject directionControlUIPrefab; // Use updated name
    [SerializeField] private Vector3 uiLocalOffset = Vector3.zero; // Typically zero if handle is root & centered

    private GameObject directionUIInstance;
    public GameObject SourcePrefab { get; private set; }
    private UnitStates currentStates;

    // Optional: Cache camera if accessed frequently
    // private Camera mainCamera;

    void Awake()
    {
        // mainCamera = Camera.main; // Example caching
        // Ensure UnitDataSO is assigned
        if (unitDataSO == null) Debug.LogError("UnitDataSO not found on Unit!", this);
    }

    // Called immediately after instantiation by TileManager.TryPlaceCharacterProvisionally
    public void InitializeAwaitDeploymentState(GameObject prefab)
    {
        this.SourcePrefab = prefab;
        currentStates = new UnitAwaitDeploymentState();
        currentStates?.StartState(this);

        if (directionControlUIPrefab != null)
        {
            directionUIInstance = Instantiate(directionControlUIPrefab, transform);
            directionUIInstance.transform.localPosition = uiLocalOffset; // Often Vector3.zero now
            directionUIInstance.transform.localRotation = Quaternion.identity;

            Canvas uiCanvas = directionUIInstance.GetComponent<Canvas>();
            if (uiCanvas != null && uiCanvas.renderMode == RenderMode.WorldSpace) { uiCanvas.worldCamera = Camera.main; }

            // *** Get the new combined script ***
            DirectionControlUI uiScript = directionUIInstance.GetComponent<DirectionControlUI>();
            if (uiScript != null) { uiScript.Initialize(this); }
            else { Debug.LogError("directionControlUIPrefab missing DirectionControlUI script!", this); }

            PlacementUIManager.Instance?.NotifyDirectionUIShown();
        }
        else { Debug.LogWarning("directionUIWorldPrefab not assigned to Unit prefab! Cannot show direction UI.", this); }
        // --- End UI Setup ---

        // Optional: Disable unwanted components here
    }

    public void SetSourcePrefab(GameObject prefab)
    {
        this.SourcePrefab = prefab;
    }

    public void ConfirmPlacement(Quaternion finalRotation)
    {
        // 1. Check if we are in the correct state to confirm
        if (currentStates is UnitAwaitDeploymentState)
        {
            // --- Placement Finalization Steps ---

            // 2. Notify manager that the Direction UI is going away
            //    Do this BEFORE destroying the UI or switching state.
            PlacementUIManager.Instance?.NotifyDirectionUIHidden(); // <-- PLACE NOTIFY HERE

            // 3. Destroy the Direction UI instance explicitly (optional but clean)
            //    Do this BEFORE switching state.
            if (directionUIInstance != null)
            {
                Destroy(directionUIInstance); // <-- PLACE UI DESTROY HERE
                directionUIInstance = null; // Clear reference
            }

            // 4. Apply the final rotation to the Unit itself
            transform.rotation = finalRotation; // <-- PLACE ROTATION SET HERE

            // 5. Get the Unit's first operational state from the factory
            UnitStates operationalState = UnitStateFactory.CreateState(unitDataSO); // <-- PLACE FACTORY CALL HERE

            // 6. Switch the Unit's state machine to the operational state
            SwitchState(operationalState); // <-- PLACE STATE SWITCH HERE

            // 7. Trigger post-placement actions (animation, enabling components, etc.)
            PlayDeploymentAnimation(); // <-- PLACE ANIMATION/ETC HERE
                                       // EnableActiveComponents(true); // Example

            Debug.Log($"{gameObject.name} placement confirmed facing {finalRotation.eulerAngles}. Switched to {operationalState?.GetType().Name}");

            // --- NOTE: DeploymentManager.RegisterDeployment() is called AFTER this
            // --- method returns, over in the DirectionDragger.OnEndDrag method.
        }
        else
        {
            Debug.LogWarning($"ConfirmPlacement called on {gameObject.name} but it was not in AwaitingDeploymentState. Current state: {currentStates?.GetType().Name}");
            // Optional: Still try to destroy the UI if it somehow exists?
            if (directionUIInstance != null) Destroy(directionUIInstance);
            PlacementUIManager.Instance?.NotifyDirectionUIHidden();
        }
    }

    // Called externally (e.g., by DirectionSelectionUI) when retreat is chosen
    public void InitiateRetreat()
    {
        // Store state information needed AFTER potential destruction
        bool wasAwaitingDirection = currentStates is UnitAwaitDeploymentState;
        GameObject prefabToRestore = SourcePrefab; // Store prefab before 'this' is potentially invalid
        Vector3 positionToUnoccupy = transform.position; // Store position before 'this' is destroyed

        UnitStates stateBeforeRetreat = currentStates; // For logging/checks

        // Exit early if state is null (something went wrong)
        if (stateBeforeRetreat == null)
        {
            Debug.LogWarning($"InitiateRetreat called on {gameObject.name} but state was null.", this);
            Destroy(gameObject); // Destroy self just in case
            return;
        }

        // --- Handle Retreat from Provisional State ---
        if (wasAwaitingDirection)
        {
            Debug.Log($"Retreat initiated for {gameObject.name} from AwaitingDeploymentState");

            // --- Cleanup and Notifications (Order Matters!) ---

            // 1. Notify PlacementUIManager UI is hidden (unblocks camera/drag ASAP)
            PlacementUIManager.Instance?.NotifyDirectionUIHidden(); // <-- PLACE NOTIFY PUI HERE

            // 2. Notify DragToScreenManager to re-enable the list icon
            //    Needs the prefab reference we stored.
            if (prefabToRestore != null)
            {
                DragToScreenManager.Instance?.HandleUnitRetreat(prefabToRestore); // <-- PLACE NOTIFY DSM HERE
            }
            else { Debug.LogWarning($"Source prefab reference missing on {gameObject.name}, cannot re-enable UI icon."); }

            // 3. Unoccupy the Tile
            //    Needs the position we stored.
            if (TileManager.Instance?.tileOccupancyCheck != null)
            {
                TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(positionToUnoccupy, false); // <-- PLACE UNOCCUPY TILE HERE
            }
            else { Debug.LogError($"Cannot unoccupy tile for {gameObject.name}, TileManager missing!"); }

            // 4. Deployment count was never incremented for this state.

            // 5. Destroy this Unit GameObject
            //    This MUST be last, as it also destroys the child UI and this script instance.
            Destroy(gameObject); // <-- PLACE DESTROY UNIT HERE
        }
        // --- Handle Retreat from Active State (Later) ---
        // else if (/* check if current state allows active retreat */)
        // {
        //     // Similar steps, but MUST call DeploymentManager.UnregisterDeployment
        //     // And potentially handle cooldowns differently for the list icon
        // }
        else
        {
            Debug.LogWarning($"InitiateRetreat called on {gameObject.name} from state {stateBeforeRetreat.GetType().Name} where retreat is not currently implemented.");
            // Optionally destroy anyway? Or just ignore? Depends on desired game rules.
            // Destroy(gameObject);
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