using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillCooldownUI : MonoBehaviour
{

    [SerializeField] private Slider slider;

    private Coroutine coroutine;

    public void StartCoolDown(float coolDownDuration)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(CooldownRoutine(coolDownDuration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {

     

        slider.value = 0f;


        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            slider.value = timer / duration;
            yield return null;
        }

        slider.value = 1f;
       

        coroutine = null;

        

    }
}
