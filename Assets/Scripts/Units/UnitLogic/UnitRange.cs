using System.Collections.Generic;
using UnityEngine;

public class UnitRange : MonoBehaviour
{
    private Unit parentUnit;
    private HashSet<EnemyMover> enemiesInRange = new HashSet<EnemyMover>();
    private BoxCollider2D boxCollider;

    [SerializeField] private SpriteRenderer rangeVisual;

    public void Initialize(Unit unit)
    {
        parentUnit = unit;
        boxCollider = GetComponent<BoxCollider2D>();

        // Hide range visual initially
        if (rangeVisual != null)
            rangeVisual.enabled = false;
    }

    // Simplified method that directly scales the transform
    public void SetRangeSize(float width, float height)
    {
        // Simply set the transform scale directly - much cleaner!
        transform.localScale = new Vector3(width, height, 1f);

        // Ensure BoxCollider2D matches this (set to 1x1 in editor)
        if (boxCollider != null)
        {
            // Reset offset to center if needed
            boxCollider.offset = new Vector2(0.5f, 0f);
        }

        // Configure visual
        if (rangeVisual != null)
        {
            // Visual settings
            rangeVisual.sortingLayerName = "Units";
            rangeVisual.sortingOrder = 5;

            // Semi-transparent
            Color rangeColor = rangeVisual.color;
            rangeColor.a = 0.5f;
            rangeVisual.color = rangeColor;
        }
    }

    // Toggle visibility of the range preview
    public void ShowRangePreview(bool show)
    {
        if (rangeVisual != null)
        {
            rangeVisual.enabled = show;
        }
    }

    // For rotation, just use transform.rotation directly
    // No need for a special ApplyRotation method with this approach

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentUnit == null || !parentUnit.IsOperational) return;

        EnemyMover enemy = other.GetComponent<EnemyMover>();
        if (enemy != null)
        {
            enemiesInRange.Add(enemy);
            parentUnit.OnEnemyEnterRange(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (parentUnit == null || !parentUnit.IsOperational) return;

        EnemyMover enemy = other.GetComponent<EnemyMover>();
        if (enemy != null)
        {
            enemiesInRange.Remove(enemy);
            parentUnit.OnEnemyExitRange(enemy);
        }
    }

    public IEnumerable<EnemyMover> GetEnemiesInRange() => enemiesInRange;
}
