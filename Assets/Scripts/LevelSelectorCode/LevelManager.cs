using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels;
    public AudioClip lobbyMusic;

    private LevelMapController levelMapController;
    void Awake()
    {
        // Find the controller once when the scene loads.
        levelMapController = FindObjectOfType<LevelMapController>();
        if (levelMapController == null)
        {
            Debug.LogError("LevelManager could not find the LevelMapController in the scene!");
        }
    }

    void Start()
    {
        ApplySavedProgress();

        if (levelMapController != null)
        {
            levelMapController.ConfigureAllLevelButtons();
        }

        if (SoundManager.Instance != null && lobbyMusic != null)
        {
            SoundManager.Instance.ChangeMusic(lobbyMusic);
        }
    }
    private void ApplySavedProgress()
    {
        // Always unlock the first level
        if (levels.Count > 0)
        {
            levels[0].isUnlocked = true;
        }

        // Update all levels based on what the ProgressManager knows
        for (int i = 1; i < levels.Count; i++)
        {
            LevelData currentLevel = levels[i];

            if (ProgressManager.Instance != null && ProgressManager.Instance.IsLevelUnlocked(currentLevel.sceneToLoad))
            {
                currentLevel.isUnlocked = true;
            }
            else
            {
                currentLevel.isUnlocked = false;
            }
        }
    }

    public void ResetLevelProgressForDebugging()
    {
        Debug.LogWarning("--- RESETTING ALL LEVEL PROGRESS ---");

        PlayerPrefs.DeleteAll();

        if (levels != null && levels.Count > 0)
        {
            LevelData firstLevel = levels[0];
            if (ProgressManager.Instance != null)
            {
                ProgressManager.Instance.UnlockLevel(firstLevel.sceneToLoad);
                Debug.Log($"RESET COMPLETE: '{firstLevel.levelName}' has been re-unlocked and saved.");
            }
        }
        else
        {
            Debug.LogError("Could not re-unlock the first level because the 'levels' list is not assigned in the LevelManager's Inspector!");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}