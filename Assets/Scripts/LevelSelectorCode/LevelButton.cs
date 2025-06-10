using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script sits on each level selection button on the map.
/// It holds the data for the level it represents and has a public function
/// to be called by the button's OnClick event in the Inspector.
/// </summary>
public class LevelButton : MonoBehaviour
{
    [Tooltip("Set this in the Inspector. It must match the level's index in the LevelManager's list.")]
    public int levelIndex;

    private LevelData levelData;

    /// <summary>
    /// Called by the LevelMapController to give this button its data and state.
    /// </summary>
    public void Setup(LevelData data)
    {
        this.levelData = data;

        // Enable or disable the button based on unlock status.
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.interactable = data.isUnlocked;
        }
    }

    /// <summary>
    /// THIS IS THE FUNCTION YOU WILL ASSIGN IN THE INSPECTOR.
    /// It acts as a bridge, calling the panel controller with this button's specific data.
    /// </summary>
    public void ShowLevelInfoPanel()
    {
        if (levelData != null)
        {
            LevelInfoPanelController.Instance.PrepareAndShowPanel(levelData);
        }
        else
        {
            Debug.LogWarning("LevelButton was clicked, but its levelData is null!", this);
        }
    }
}