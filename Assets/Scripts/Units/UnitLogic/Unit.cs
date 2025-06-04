// Unit.cs (Updated to Manage World Space UI)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Unit Data")]
    public UnitDataSO unitDataSO;
    public Animator animator;
    public bool facingRight = true;
    private IUnitStates currentState;

    //Prefab for arrow and heal
    public GameObject ArrowPrefab;
    public GameObject HealEffectPrefab;

    // Assign max and current health here
    public int currentHealth;
    public int MaxHealth => unitDataSO.maxHealth;


    [Header("World Space UI")]
    [Tooltip("Assign the World Space Canvas Prefab with DirectionControlUI script")]
    [SerializeField] private GameObject directionControlUIPrefab; // Use updated name
    [SerializeField] private Vector3 uiLocalOffset = Vector3.zero; // Typically zero if handle is root & centered

    [Header("Range Settings")]
    [SerializeField] private UnitRange rangeComponent;
    [SerializeField] private float rangeWidthTiles = 3f;
    [SerializeField] private float rangeHeightTiles = 5f;
    [SerializeField] private float tileSize = 1f;

    private GameObject directionUIInstance;
    public GameObject SourcePrefab { get; private set; }
    private UnitStates currentStates;

    //List of friend and foe
    private List<EnemyPathFollower> blockedEnemies = new List<EnemyPathFollower>();
    public List<Unit> alliesInRangeList = new List<Unit>();


    public bool IsOperational { get; private set; } = false;
    public int BlockCount => unitDataSO != null ? unitDataSO.blockCount : 1;

    [SerializeField] private HealthBarSlider healthBarSlider;


    void OnEnable()
    {
        StartCoroutine(AttackEnemiesInRangeRoutine());
    }

    void OnDisable()
    {
        StopCoroutine(AttackEnemiesInRangeRoutine());
    }

    // Optional: Cache camera if accessed frequently
    // private Camera mainCamera;

    void Awake()
    {
        animator = GetComponent<Animator>();
        healthBarSlider = GetComponentInChildren<HealthBarSlider>();
        // mainCamera = Camera.main; // Example caching
        // Ensure UnitDataSO is assigned
        if (unitDataSO == null) Debug.LogError("UnitDataSO not found on Unit!", this);
        currentHealth = unitDataSO != null ? unitDataSO.maxHealth : 1;
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
        if (rangeComponent != null)
        {
            rangeComponent.Initialize(this);
            rangeComponent.SetRangeSize(rangeWidthTiles * tileSize, rangeHeightTiles * tileSize);
            rangeComponent.ShowRangePreview(false); // Initially hidden, will show on drag
        }
    }

    public void SetSourcePrefab(GameObject prefab)
    {
        this.SourcePrefab = prefab;
    }

    public void ConfirmPlacement(Quaternion finalRotation)
    {
        UnitRangeArcher archer = GetComponent<UnitRangeArcher>();
        if (archer != null)
        {
            archer.Initialize(this);
        }

        UnitMedic medic = GetComponent<UnitMedic>();
        if(medic != null)
        {
            medic.Initialize(this);
        }


        IsOperational = true;
        // 1. Check if we are in the correct state to confirm
        if (currentStates is UnitAwaitDeploymentState)
        {
            // --- DP Check and Spend ---
            if (unitDataSO != null && DPManager.Instance != null)
            {
                int unitDP = unitDataSO.DP;
                if (!DPManager.Instance.CanSpendDP(unitDP))
                {
                    Debug.LogWarning("Not enough DP to confirm deployment!");
                    // Optionally destroy the provisional unit or show a warning
                    Destroy(gameObject);
                    return;
                }
                DPManager.Instance.SpendDP(unitDP);
            }

            // --- Placement Finalization Steps ---

            // 2. Notify manager that the Direction UI is going away
            PlacementUIManager.Instance?.NotifyDirectionUIHidden();

            // 3. Destroy the Direction UI instance explicitly (optional but clean)
            if (directionUIInstance != null)
            {
                var directionUI = directionUIInstance.GetComponent<DirectionControlUI>();
                if (directionUI != null)
                {
                    directionUI.HideAllControls();
                }
            }

            // Update range rotation to match final unit rotation
            if (rangeComponent != null)
            {
                rangeComponent.transform.rotation = finalRotation;
            }

            UnitStates operationalState = UnitStateFactory.CreateState(unitDataSO);

            SwitchState(operationalState);

            // 7. Trigger post-placement actions (animation, enabling components, etc.)
            PlayDeploymentAnimation();
        }
        else
        {
            Debug.LogWarning($"ConfirmPlacement called on {gameObject.name} but it was not in AwaitingDeploymentState. Current state: {currentStates?.GetType().Name}");
            if (directionUIInstance != null) Destroy(directionUIInstance);
            PlacementUIManager.Instance?.NotifyDirectionUIHidden();
        }
        ScanForInitialEnemies();
    }

    // Called externally (e.g., by DirectionSelectionUI) when retreat is chosen
    public void InitiateRetreat()
    {
        // Store state information needed AFTER potential destruction
        bool wasAwaitingDirection = currentStates is UnitAwaitDeploymentState;
        GameObject prefabToRestore = SourcePrefab;
        Vector3 positionToUnoccupy = transform.position;

        UnitStates stateBeforeRetreat = currentStates;

        if (stateBeforeRetreat == null)
        {
            Debug.LogWarning($"InitiateRetreat called on {gameObject.name} but state was null.");
            Destroy(gameObject);
            return;
        }

        if (wasAwaitingDirection)
        {

            PlacementUIManager.Instance?.NotifyDirectionUIHidden();
            if (prefabToRestore != null)
            {
                DragToScreenManager.Instance?.HandleUnitRetreat(prefabToRestore);
            }
            else { Debug.LogWarning($"Source prefab reference missing on {gameObject.name}, cannot re-enable UI icon."); }

            // Unoccupy the tile
            if (TileManager.Instance?.tileOccupancyCheck != null)
            {
                TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(positionToUnoccupy, false);
            }
            else { Debug.LogError($"Cannot unoccupy tile for {gameObject.name}, TileManager missing!"); }

            Destroy(gameObject);
        }
        // --- Handle Retreat from Active/Operational State ---
        else
        {

            // Get the icon for the unit (assumes you have a way to get it, e.g., from UnitDataSO)
            Sprite unitIcon = unitDataSO != null ? unitDataSO.icon : null; // Add an 'icon' field to UnitDataSO if needed
            float cooldown = unitDataSO != null ? unitDataSO.redeployCooldown : 10f; // Or use a dedicated redeployCooldown

            RedeploymentManager.Instance.AddToWaitList(SourcePrefab, unitIcon, cooldown);

            if (unitDataSO != null && DPManager.Instance != null)
            {
                DPManager.Instance.GainDP(unitDataSO.DP);
            }

            // Clear any selection first
            UnitSelectionManager.Instance?.ClearSelection();

            // Unregister deployment with deployment manager
            if (prefabToRestore != null)
            {
                DeploymentManager.Instance?.UnregisterDeployment(prefabToRestore);
                // Re-enable the icon for deployment
                DragToScreenManager.Instance?.HandleUnitRetreat(prefabToRestore);
            }

            // Unoccupy the tile
            if (TileManager.Instance?.tileOccupancyCheck != null)
            {
                TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(positionToUnoccupy, false);
            }

            Destroy(gameObject);
        }
    }


    // UPDATE FUNCTION IS HERE NIBA
    private void Update()
    {
        currentStates?.UpdateState(this);
        UpdateAnimatorState();
        UpdateAlliesInRange();

        CleanEnemyList(); // prevent null enemy taking up space in the list

    }


    // Keep SwitchState and other helpers...
    public void SwitchState(UnitStates newState) { /* ... */ if (newState == null) return; currentStates?.ExitState(this); currentStates = newState; currentStates?.StartState(this); }
    private void PlayDeploymentAnimation() { Debug.Log($"Playing Deployment Animation for {gameObject.name}"); }



    //enemy in range
    public List<EnemyPathFollower> enemiesInRangeList = new List<EnemyPathFollower>();
    private EnemyPathFollower CurrentTarget => enemiesInRangeList.Count > 0 ? enemiesInRangeList[0] : null;


    public void OnEnemyEnterRange(EnemyPathFollower enemy)
    {
        if (!IsOperational || enemiesInRangeList.Contains(enemy)) return;

        enemiesInRangeList.Add(enemy);
    }

    public void OnEnemyExitRange(EnemyPathFollower enemy)
    {
        if (enemiesInRangeList.Remove(enemy))
        {
            if (enemy == CurrentTarget)
            {
                // Target removed — auto-rotate to new first enemy
                UpdateAnimatorState();
            }
        }
    }

    // Add this method to Unit.cs
    public UnitStates GetCurrentState()
    {
        return currentStates;
    }
    public bool CanBlockMore()
    {
        return blockedEnemies.Count < BlockCount;
    }

    public void AddBlockedEnemy(EnemyPathFollower enemy)
    {
        if (!blockedEnemies.Contains(enemy))
            blockedEnemies.Add(enemy);
    }

    public void RemoveBlockedEnemy(EnemyPathFollower enemy)
    {
        blockedEnemies.Remove(enemy);
    }

 


    public void DealDamage()
    {
        if (!IsOperational || enemiesInRangeList.Count == 0) return;

        var target = CurrentTarget;
        if (target == null) return;

        int damage = unitDataSO != null ? unitDataSO.attackDamage : 1;
        var enemyHealth = target.GetComponent<Entity>();
        

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            if (enemyHealth.CurrentHealth <= 0)
            {
                enemiesInRangeList.Remove(target); // Remove the one we damaged and confirmed dead
            }
        }
    }

    private void UpdateAnimatorState()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", CurrentTarget != null);
        }
    }

    public void TriggerHealingAnim()
    {
        if (animator != null)
        {
            animator.SetBool("isHealing", true);
        }
    }

    void ScanForInitialEnemies()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + direction * unitDataSO.attackOffset.x;
        float radius = unitDataSO.attackRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<EnemyPathFollower>();
            if (enemy != null)
            {
                OnEnemyEnterRange(enemy);
            }
        }
    }



    private IEnumerator AttackEnemiesInRangeRoutine()
    {
       while (true)
    {
        if (IsOperational)
        {
            UpdateAnimatorState(); // triggers animation if enemy present
        }
        float interval = unitDataSO != null ? unitDataSO.attackInterval : 1.0f;
        yield return new WaitForSeconds(interval);
    }
    }

    //Add foe to list
    void UpdateAlliesInRange()
    {
        alliesInRangeList.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, unitDataSO.attackRange);
        foreach (var col in colliders)
        {
            Unit ally = col.GetComponent<Unit>();
            if (ally != null && ally != this && ally.IsOperational)
            {
                alliesInRangeList.Add(ally);
            }
        }
    }



    public bool isDead;
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBarSlider.UpdateHealth(currentHealth, unitDataSO.maxHealth);
        if (currentHealth <= 0)
        {
            Die();
            isDead = true;
        }
    }

    //Healing bitch
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        // trigger heal VFX sound
    }

    private void Die()
    {
        // Add any cleanup logic here (e.g., notify managers, play animation)
        Destroy(gameObject);
    }

    private void CleanEnemyList()
    {
        enemiesInRangeList.RemoveAll(enemy => enemy == null);
    }


    private void OnDrawGizmosSelected()
    {
        if (unitDataSO == null)
            return;

        Gizmos.color = Color.red;

        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 center = (Vector2)transform.position + direction * unitDataSO.attackOffset.x;

        Gizmos.DrawWireSphere(center, unitDataSO.attackRange);
    }

}