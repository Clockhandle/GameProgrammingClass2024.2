using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarSlider : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider damageIndicatorSlider;

    private float targetNormalizedValue = 1f;
    [SerializeField] private float indicatorSpeed = 1.5f;
    public void UpdateHealth(float currentValue, float maxValue)
    {
        targetNormalizedValue = currentValue / maxValue;
        healthBarSlider.value = currentValue/ maxValue;
    }
    private void Update()
    {
        if (damageIndicatorSlider.value > targetNormalizedValue)
        {
            damageIndicatorSlider.value = Mathf.Lerp(
                damageIndicatorSlider.value,
                targetNormalizedValue,
                Time.deltaTime * indicatorSpeed
            );
        }
        else
        {
            damageIndicatorSlider.value = targetNormalizedValue;
        }
    }
}

