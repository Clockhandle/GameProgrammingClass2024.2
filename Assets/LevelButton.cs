using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelButton : MonoBehaviour
{
    [Tooltip("Set this in the Inspector. It must match the level's index in the LevelManager's list.")]
    public int levelIndex; // The key new variable!

    public Button buttonComponent;
    public LevelData levelData;
    private bool isLocked;

    // Setup is now called by the LevelMapController
    public void Setup(LevelData data)
    {
        levelData = data;
        isLocked = !data.isUnlocked;


        if (isLocked)
        {
            buttonComponent.interactable = false;
        }
        else
        {
            buttonComponent.interactable = true;
            // IMPORTANT: Only add the listener if the level is unlocked
            buttonComponent.onClick.AddListener(LoadLevel);
        }
    }

    private void LoadLevel()
    {
        //Debug.Log("Loading level: " + levelData.levelName);
        SceneManager.LoadScene(levelData.sceneToLoad);
    }
}
