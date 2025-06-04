using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected EnemyDataSO enemyData;

    private SpriteRenderer spriteRenderer;
    private float fadeDuration = 1f; // How long to fade out
    private float fadeTimer;
    private Color originalColor;

    public DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, EnemyDataSO enemyData) : base(entity, stateMachine, animBoolName)
    {
        this.enemyData = enemyData; 
    }

    public override void Enter()
    {
        base.Enter();
        //Hande death particle and effect
        if (entity.TryGetComponent<SpriteRenderer>(out spriteRenderer))
        {
            originalColor = spriteRenderer.color;
        }

        fadeTimer = 0f;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (spriteRenderer != null)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, fadeTimer / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            if (alpha <= 0.01f)
            {
                GameObject.Destroy(entity.gameObject); // Remove from scene
            }
        }
    }
}
