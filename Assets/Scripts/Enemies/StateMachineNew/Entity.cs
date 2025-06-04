using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{


    public EnemyPathFollower pathFollower { get; private set; }

    protected int currentHealth;
    public int CurrentHealth => currentHealth; // read only
    protected bool isDead = false;
    public virtual bool IsTrulyDead => isDead;

    public int facingDir { get; private set; }

    public Animator anim { get; private set; }
    protected Unit currentTarget;
    public Unit CurrentTar => currentTarget;

    public List<Unit> detectedTargets = new List<Unit>();

    public FiniteStateMachine stateMachine;

    public EnemyDataSO enemyDataSO;

    public AnimationToStateMachine atsm { get; private set; }


    [SerializeField] HealthBarSlider healthBarSlider;



    public virtual void Start()
    {
        healthBarSlider = GetComponentInChildren<HealthBarSlider>();
        anim  = GetComponent<Animator>();
        stateMachine = new FiniteStateMachine();
        pathFollower = GetComponent<EnemyPathFollower>();
        facingDir = 1;
        atsm = GetComponent<AnimationToStateMachine>();

        currentHealth = enemyDataSO != null ? enemyDataSO.maxHealth : 1;

    }
    public virtual void Update()
    {
        stateMachine.currentState.LogicUpdate();

        healthBarSlider.UpdateHealth(currentHealth, this.enemyDataSO.maxHealth);
    }

    public virtual void Move(bool canMove)
    {
        pathFollower.isPaused = !canMove;
    }
    public virtual bool DetectUnit()
    {
        if (currentTarget != null) return true;

        return false;
    }

    public virtual bool DetectLongRange()
    {
        detectedTargets.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemyDataSO.longRange, LayerMask.GetMask("Unit"));
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Unit unit))
            {
                if (!detectedTargets.Contains(unit))
                {
                    detectedTargets.Add(unit);
                }
            }
        }
        ValidateTargetList();
        return currentTarget != null;
    }

    public void ValidateTargetList()
    {
        // Remove null or dead units
        detectedTargets.RemoveAll(unit => unit == null);

        // Assign the first valid target
        currentTarget = detectedTargets.Count > 0 ? detectedTargets[0] : null;
    }


    public virtual void SetTarget(Unit unit)
    {
        currentTarget = unit;
    }

    public virtual void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        // healthBarSlider.UpdateHealth(currentHealth, this.enemyDataSO.maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }



    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
       // animator?.SetBool("isAttacking", false);
        // Unified: Notify GameManager that this enemy is defeated (slain or reached goal)
        GameManager.Instance?.OnEnemyDefeated();

        // Add death effects, drop loot, etc.
        //Destroy(gameObject);
    }

    public void DealDamageToUnit()
    {
        if (currentTarget != null)
        {
            currentTarget.TakeDamage(enemyDataSO.damage);
        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (currentTarget == null && other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null)
            {
                SetTarget(unit);
                Debug.Log("Enemy detected unit: " + unit.name);
            }
        }
    }
   

    public virtual void Flip()
    {
        facingDir *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

   

}
