using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<LevelData> levels;
    public AudioClip lobbyMusic;
    void Start()
    {
        ApplySavedProgress();

        //Will change later
        SoundManager.Instance.ChangeMusic(lobbyMusic);
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


}