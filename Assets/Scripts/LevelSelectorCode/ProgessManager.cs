using UnityEngine;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public LevelData selectedLevel;
    public List<Cards> selectedCards;

    public string sceneOfNextLevelToUnlock;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        selectedCards = new List<Cards>();
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