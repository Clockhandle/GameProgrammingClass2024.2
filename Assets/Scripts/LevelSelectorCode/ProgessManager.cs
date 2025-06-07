using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    // Add a reference to the level list asset if you have one, 
    // or just handle unlocking by scene name. Let's use scene names for simplicity.

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockLevel(string levelSceneNameToUnlock)
    {
        PlayerPrefs.SetInt(levelSceneNameToUnlock, 1);
        PlayerPrefs.Save();
        Debug.Log($"Level Unlocked and Saved: {levelSceneNameToUnlock}");
    }

    public bool IsLevelUnlocked(string levelSceneName)
    {
        return PlayerPrefs.GetInt(levelSceneName, 0) == 1;
    }
}