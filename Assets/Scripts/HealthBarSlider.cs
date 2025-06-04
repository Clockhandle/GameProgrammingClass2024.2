using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarSlider : MonoBehaviour
{
    [SerializeField] private Slider healthBarSlider;

    public void UpdateHealth(float currentValue, float maxValue)
    {
        healthBarSlider.value = currentValue/ maxValue;
    }
    private void Update()
    {
        
    }
}
