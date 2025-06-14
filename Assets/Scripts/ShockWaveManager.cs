using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


    public class ShockWaveManager : MonoBehaviour
    {
        [SerializeField] private float duration = 0.75f;
        private Coroutine shockwavecoronTine;
        private Material material;
        private int waveDistance = Shader.PropertyToID("_WaveDistanceStrength");

        private void Awake()
        {
            material = GetComponent<SpriteRenderer>().material;

        }

        public void CallShowckWave()
        {
            shockwavecoronTine = StartCoroutine(ShowckWave(-.1f, 1f));

        }

        private IEnumerator ShowckWave(float startPos, float endPos)
        {
            material.SetFloat(waveDistance, startPos);
            float lerpAmount = 0;
            float elapsedTime = 0;
            while(elapsedTime< duration)
            {
                elapsedTime += Time.deltaTime;
                lerpAmount = Mathf.Lerp(startPos, endPos, (elapsedTime/duration));
                material.SetFloat(waveDistance, lerpAmount);

                yield return null;
            }
        }

    public void AutoDestroy(float delay)
    {
        StartCoroutine(DestroyAfter(delay));
    }

    private IEnumerator DestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

}

