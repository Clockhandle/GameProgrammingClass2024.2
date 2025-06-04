using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyDataSO enemyData;

    protected int currentHealth;
    public int CurrentHealth => currentHealth; // read only
    protected bool isDead = false;

    protected Animator animator;
    public Animator animator_refe;
    
    
    protected Unit currentTarget;
   public Unit CurrentTarget => currentTarget;

    [SerializeField] HealthBarSlider healthBarSlider;


    // ENemy PSepcial Sate
    public SpecialEnemyStateMachine stateMachine;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        healthBarSlider = GetComponentInChildren<HealthBarSlider>();

       
            stateMachine = GetComponent<SpecialEnemyStateMachine>();
        

        if (enemyData == null)
            Debug.LogError("EnemyDataSO not assigned!", this);

        currentHealth = enemyData != null ? enemyData.maxHealth : 1;

        animator_refe = animator;
    }

    protected virtual void Start()
    {
        OnSpawn();
        var startingState = GetComponent<WalkStateSpecial>(); // or IdleStateSpecial, etc.

        if (startingState != null)
        {
            stateMachine.Initialize(startingState);
        }
    }

    protected virtual void Update()
    {
        OnUpdate();

        //if (currentTarget != null && !isDead)
        //{
        //    animator?.SetBool("isAttacking", true);
        //    Debug.Log("Detect Unit");
        //}
        //else
        //{
        //    animator?.SetBool("isAttacking", false);
        //}
    }


    protected virtual void OnDestroy()
    {
        OnDeath();
    }

    // --- Common Logic ---

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        healthBarSlider.UpdateHealth(currentHealth, this.enemyData.maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        animator?.SetBool("isAttacking", false);
        // Unified: Notify GameManager that this enemy is defeated (slain or reached goal)
        GameManager.Instance?.OnEnemyDefeated();

        // Add death effects, drop loot, etc.
        Destroy(gameObject);
    }

    public void SetTarget(Unit unit)
    {
        currentTarget = unit;
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (currentTarget == null && other.gameObject.CompareTag("Unit"))
    //    {
    //        Unit unit = other.gameObject.GetComponent<Unit>();
    //        if (unit != null)
    //        {
    //            SetTarget(unit);
    //            Debug.Log("Target acquired: " + unit.name);
    //        }
    //    }
    //}

    public void DealDamageToUnit()
    {
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(enemyData.damage);
        }
    }

    // --- Extensible Hooks ---

    protected virtual void OnSpawn() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDeath() { }
}
