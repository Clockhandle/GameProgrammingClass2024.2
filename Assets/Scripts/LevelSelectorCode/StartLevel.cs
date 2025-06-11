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

    // A private reference to the LevelManager in the scene.
    private LevelManager levelManager;

    void Awake()
    {
        // It's best to find the LevelManager once when the object wakes up.
        // This is more efficient than searching for it every time the button is clicked.
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
        // 1. Get the data for the level that is currently displayed in the panel.
        LevelData currentLevel = LevelInfoPanelController.Instance.CurrentLevelData;

        // 2. Safety check: make sure we actually have level data.
        if (currentLevel == null)
        {
            Debug.LogError("StartLevel button clicked, but no level data found in the panel controller!", this);
            return;
        }

        // 3. Store the current level's data in our persistent ProgressManager.
        ProgressManager.Instance.selectedLevel = currentLevel;

        // --- THIS IS THE KEY NEW LOGIC ---

        // 4. Find this level in the LevelManager's master list to determine its position.
        int currentIndex = levelManager.levels.IndexOf(currentLevel);

        // 5. Check if there is a next level in the sequence.
        if (currentIndex >= 0 && currentIndex + 1 < levelManager.levels.Count)
        {
            // If so, get the data for the next level.
            LevelData nextLevel = levelManager.levels[currentIndex + 1];

            // Store JUST THE SCENE NAME of the next level for the GameManager to use later.
            ProgressManager.Instance.sceneOfNextLevelToUnlock = nextLevel.sceneToLoad;

            // --- ADDED DEBUG LOG ---
            Debug.Log($"CARGO LOADED: Next level to unlock is '{nextLevel.sceneToLoad}'");
        }
        else
        {
            // There is no next level, so clear the string to prevent errors.
            ProgressManager.Instance.sceneOfNextLevelToUnlock = null;

            // --- ADDED DEBUG LOG ---
            Debug.Log("CARGO LOADED: This is the last level. No next level to unlock.");
        }
        // --- END NEW LOGIC ---

        // 6. Load the character selection scene.
        SceneManager.LoadScene(characterSelectSceneName);
    }
}