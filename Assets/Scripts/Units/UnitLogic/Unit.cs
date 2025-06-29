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

    // Add a runtime field to track remaining deployments for this unit instance
    private int remainingDeployments;

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
    private UnitAudio unitAudio;

    //List of friend and foe
    private List<EnemyPathFollower> blockedEnemies = new List<EnemyPathFollower>();
    public List<Unit> alliesInRangeList = new List<Unit>();


    public bool IsOperational { get; private set; } = false;
    public int BlockCount => unitDataSO != null ? unitDataSO.blockCount : 1;

    [SerializeField] private HealthBarSlider healthBarSlider;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private GameObject deathEffectPrefab;


    [Header("Deployment Effect")]
    [SerializeField] private GameObject deploymentEffectPrefab; 
    [SerializeField] private Transform deploymentEffectSpawnPoint;


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

    private UnitRangeGeneral rangeGeneral;

    void Awake()
    {

        rangeGeneral = GetComponent<UnitRangeGeneral>();
        // get parent transform
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        healthBarSlider = GetComponentInChildren<HealthBarSlider>();
        // mainCamera = Camera.main; // Example caching
        // Ensure UnitDataSO is assigned
        if (unitDataSO == null) Debug.LogError("UnitDataSO not found on Unit!", this);
        currentHealth = unitDataSO != null ? unitDataSO.maxHealth : 1;
        unitAudio = GetComponent<UnitAudio>();
        remainingDeployments = unitDataSO != null ? unitDataSO.maxNumberOfDeployments : 1;
    }

    // Called immediately after instantiation by TileManager.TryPlaceCharacterProvisionally
    public void InitializeAwaitDeploymentState(GameObject prefab)
    {
        this.SourcePrefab = prefab;
        currentStates = new UnitAwaitDeploymentState();
        currentStates?.StartState(this);

        // Initialize runtime deployment count for this instance
        remainingDeployments = unitDataSO != null ? unitDataSO.maxNumberOfDeployments : 1;

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

        UnitRangeGeneral general = GetComponent<UnitRangeGeneral>();
        if (general != null)
        {
            general.Initialize(this); 
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
                // Check runtime deployment count BEFORE spending DP
                if (remainingDeployments <= 0)
                {
                    Debug.LogWarning("No deployments left for this unit!");
                    DragToScreenManager.Instance?.HandleUnitRetreat(SourcePrefab);
                    TileManager.Instance?.tileOccupancyCheck?.SetTileToOccupied(transform.position, false);
                    Destroy(gameObject);
                    return;
                }
                if (!DPManager.Instance.CanSpendDP(unitDP))
                {
                    Debug.LogWarning("Not enough DP to confirm deployment!");
                    DragToScreenManager.Instance?.HandleUnitRetreat(SourcePrefab);
                    TileManager.Instance?.tileOccupancyCheck?.SetTileToOccupied(transform.position, false);
                    Destroy(gameObject);
                    return;
                }
                DPManager.Instance.SpendDP(unitDP);
                //remainingDeployments -= 1; // Only decrement after successful DP spend
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

          

            //Instantiate Deployment effect
            GameObject deployedEffect = Instantiate(deploymentEffectPrefab, deploymentEffectSpawnPoint.position, deploymentEffectSpawnPoint.rotation);
            Destroy(deployedEffect, .5f);



            // 7. Trigger post-placement actions (animation, enabling components, etc.)
            PlayDeploymentAnimation();
        }
        else
        {
            Debug.LogWarning($"ConfirmPlacement called on {gameObject.name} but it was not in AwaitingDeploymentState. Current state: {currentStates?.GetType().Name}");
            if (directionUIInstance != null) Destroy(directionUIInstance);
            PlacementUIManager.Instance?.NotifyDirectionUIHidden();
        }
        //ScanForInitialEnemies();
    }

    // Optionally, expose a method to get remaining deployments for UI
    public int GetRemainingDeployments()
    {
        return remainingDeployments;
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

            RedeploymentManager.Instance.AddToWaitList(SourcePrefab, unitIcon, cooldown, entry => {

                // When retreat updadt text
                UnitIconData iconData = UnitIconData.GetIconDataByPrefab(entry.unitPrefab);
                if (iconData != null)
                {
                    var deploymentText = iconData.GetComponentInChildren<DeploymentAmountText>();
                    if (deploymentText != null)
                    {
                        deploymentText.UpdateDeployCountText();
                    }
                }

            });



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
        healthBarSlider.UpdateHealth(currentHealth, unitDataSO.maxHealth);

        CleanEnemyList(); // prevent null enemy taking up space in the list

    }


    // Keep SwitchState and other helpers...
    public void SwitchState(UnitStates newState) { /* ... */ if (newState == null) return; currentStates?.ExitState(this); currentStates = newState; currentStates?.StartState(this); }
    private void PlayDeploymentAnimation() { Debug.Log($"Playing Deployment Animation for {gameObject.name}"); }



    //enemy in range
    public List<EnemyPathFollower> enemiesInRangeList = new List<EnemyPathFollower>();
    protected EnemyPathFollower CurrentTarget => enemiesInRangeList.Count > 0 ? enemiesInRangeList[0] : null;


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
                // Target removed � auto-rotate to new first enemy
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

    //void ScanForInitialEnemies()
    //{
    //    Vector2 direction = facingRight ? Vector2.right : Vector2.left;
    //    Vector2 center = (Vector2)transform.position + direction * unitDataSO.attackOffset.x;
    //    float radius = unitDataSO.attackRange;

    //    Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);
    //    foreach (var hit in hits)
    //    {
    //        var enemy = hit.GetComponent<EnemyPathFollower>();
    //        if (enemy != null)
    //        {
    //            OnEnemyEnterRange(enemy);
    //        }
    //    }
    //}



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
        unitAudio.PlayHurtSound();
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
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        if (animator != null)
        {
            animator.SetTrigger("Die");
            unitAudio.PlayDeathSound();
        }

        GameObject effect =  Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f);

        StartCoroutine(DestroyAfterDeathAnimation());
    }

    private IEnumerator DestroyAfterDeathAnimation()
    {

        yield return new WaitForSeconds(2f); // Wait for animation to finish
        TileManager.Instance.tileOccupancyCheck.SetTileToOccupied(transform.position, false); // Unoccupy the tile
        Destroy(gameObject);
      
    }



    private void CleanEnemyList()
    {
        enemiesInRangeList.RemoveAll(enemy => enemy == null);
    }


  

    //Skill Buff UNit only
    public void TryActivateBuffSkill()
    {
        if (this is BuffGeneralUnit buffUnit)
        {
            buffUnit.ActivateBuffSkill();

        }
    }

    //Skill Dash UNit only
    public void TryActivateDashSkill()
    {
        if (this is DashGeneralUnit dashUnit)
        {
            dashUnit.ActivateDashSkill();
        }
    }

    public void TryActivateChargeSkill()
    {
        if (this is ChargeGeneralUnit chargeUnit)
        {
            chargeUnit.ActiveChargeSkill();
        }
    }

    //Skill Shoot for range general
    public void TryActivateBurstSkill()
    {
        if (rangeGeneral != null)
        {
            rangeGeneral.ActivateBurstSkill();
        }
    }

   

    //Hanle Fllip Unit

    public void SetFacingDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }

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