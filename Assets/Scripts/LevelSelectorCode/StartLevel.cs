using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script is attached to the "Start Level" button inside the LevelInfoPanel.
/// Its job is to gather all necessary data for the upcoming gameplay session,
/// store it in the persistent ProgressManager, and then load the next scene.
/// </summary>
public class StartLevel : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The name of the scene to load after selecting a level.")]
    [SerializeField] private string characterSelectSceneName = "CharSelection";

    private LevelManager levelManager;

    void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("StartLevel could not find the LevelManager in the scene!", this);
        }
    }

    /// <summary>
    /// This is the public method you will assign to the button's OnClick event.
    /// </summary>
    public void ProceedToCharacterSelect()
    {
        LevelData currentLevel = LevelInfoPanelController.Instance.CurrentLevelData;

        if (currentLevel == null)
        {
            Debug.LogError("StartLevel button clicked, but no level data found in the panel controller!", this);
            return;
        }

        ProgressManager.Instance.selectedLevel = currentLevel;

        int currentIndex = levelManager.levels.IndexOf(currentLevel);

        if (currentIndex >= 0 && currentIndex + 1 < levelManager.levels.Count)
        {
            LevelData nextLevel = levelManager.levels[currentIndex + 1];
            ProgressManager.Instance.sceneOfNextLevelToUnlock = nextLevel.sceneToLoad;
            Debug.Log($"CARGO LOADED: Next level to unlock is '{nextLevel.sceneToLoad}'");
        }
        else
        {
            ProgressManager.Instance.sceneOfNextLevelToUnlock = null;
            Debug.Log("CARGO LOADED: This is the last level. No next level to unlock.");
        }
        SceneManager.LoadScene(characterSelectSceneName);
    }
}