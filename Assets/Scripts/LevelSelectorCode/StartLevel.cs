using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The name of the scene to load after selecting a level.")]
    [SerializeField] private string characterSelectSceneName = "CharSelection";

    /// <summary>
    /// This is the public method you will assign to the button's OnClick event.
    /// It gets the chosen level's data from the panel controller, saves it, and loads the next scene.
    /// </summary>
    public void ProceedToCharacterSelect()
    {

        // 1. Get the data for the level that is currently displayed in the panel.
        LevelData levelToLoad = LevelInfoPanelController.Instance.CurrentLevelData;

        // 2. Safety check: make sure we actually have level data.
        if (levelToLoad == null)
        {
            Debug.LogError("StartLevel button was clicked, but no level data was found in the panel controller!");
            return;
        }

        // 3. Store this data in our persistent ProgressManager (the "cargo ship").
        ProgressManager.Instance.selectedLevel = levelToLoad;

        // 4. Load the character selection scene.
        SceneManager.LoadScene(characterSelectSceneName);
    }
}
