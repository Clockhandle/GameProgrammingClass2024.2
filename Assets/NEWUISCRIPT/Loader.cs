using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour
    {

    }
    public enum Scene
    {
        GameScene,
        Loading,
        MainMenu,
    }

    private static Action onLoaderCallBack;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(string scene)
    {
        onLoaderCallBack = () =>
        {

            GameObject LoadingGame = new GameObject("Loading Game Object");
            LoadingGame.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
           // LoadSceneAsync(scene);
        };
        SceneManager.LoadScene("LoadingMenu");
    }

    private static IEnumerator LoadSceneAsync(string scene)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene);
        loadingAsyncOperation.allowSceneActivation = false;

        float minLoadTime = 2f; // seconds
        float timer = 0f;

        // Wait until Unity reports progress >= 0.9f
        while (loadingAsyncOperation.progress < 0.9f)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        // Wait until both: progress bar reaches 1 AND minLoadTime passes
        while (Loader.GetLoadingProgress() < 1f || timer < minLoadTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        loadingAsyncOperation.allowSceneActivation = true;
    }


    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            // Normalize 0.0 to 0.9 → 0.0 to 1.0
            return Mathf.Clamp01(loadingAsyncOperation.progress / 0.9f);
        }
        else
        {
            return 1f;
        }
    }


    public static void LoaderCallBack()
    {
        //Trigger after first updatee, let the scene refresh
        if (onLoaderCallBack != null)
        {
            onLoaderCallBack();
            onLoaderCallBack = null;
        }
    }
}
