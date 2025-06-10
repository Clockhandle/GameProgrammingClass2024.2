using UnityEngine;
using UnityEngine.UIElements;

public class LoadingMenu : MonoBehaviour
{
    private ProgressBar progressBar;
    private float progressTarget = 0f;
    private float fillSpeed = 1f;

    private void Awake()
    {
        progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>("loading_progress_bar");
        progressBar.value = 0f;
    }
    private void Update()
    {
        float target = Mathf.Clamp01(Loader.GetLoadingProgress() / 0.9f); // normalize to 0–1
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, Time.deltaTime * fillSpeed);
    }
}
