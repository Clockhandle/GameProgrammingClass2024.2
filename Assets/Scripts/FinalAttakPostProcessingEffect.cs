using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FinalAttakPostProcessingEffect : MonoBehaviour
    {
    [Header("Post-Processing Volume")]
    [SerializeField] private Volume globalVolume;

    [Header("Effect Settings")]
    [SerializeField] private float lensDistortionAmount = -0.4f;
    [SerializeField] private float chromaticAberrationAmount = 1.0f;
    [SerializeField] private float revertDuration = 1.0f;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;

    private float originalDistortion;
    private float originalChromatic;

    private Coroutine revertCoroutine;


    private void Awake()
    {
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out chromaticAberration);

            if (lensDistortion != null)
                originalDistortion = lensDistortion.intensity.value;

            if (chromaticAberration != null)
                originalChromatic = chromaticAberration.intensity.value;
        }
    }

    public void ActivateEffect()
    {
        Debug.Log("POostProcessing is called");
        if (lensDistortion != null)
            lensDistortion.intensity.value = lensDistortionAmount;

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = chromaticAberrationAmount;

        // Start revert coroutine
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);

        revertCoroutine = StartCoroutine(RevertEffect());
    }

    private IEnumerator RevertEffect()
    {
        float elapsed = 0f;

        float startDistortion =  lensDistortion.intensity.value; 
        float startChromatic =  chromaticAberration.intensity.value;

        while (elapsed < revertDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / revertDuration;

          
           lensDistortion.intensity.value = Mathf.Lerp(startDistortion, originalDistortion, t);

           
           chromaticAberration.intensity.value = Mathf.Lerp(startChromatic, originalChromatic, t);

            yield return null;
        }

        
    }




}

