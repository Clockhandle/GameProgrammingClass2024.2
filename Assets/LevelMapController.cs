using UnityEngine;

public class LevelMapController : MonoBehaviour
{
    [Tooltip("A direct reference to the LevelManager in your scene. The LevelManager holds the master list of all levels.")]
    [SerializeField] private LevelManager levelManager;

    void Start()
    {
        if (levelManager == null)
        {
            Debug.LogError("LevelMapController: The 'LevelManager' reference has not been set in the Inspector! Aborting setup.");
            return;
        }
        ConfigureAllLevelButtons();
    }

    private void ConfigureAllLevelButtons()
    {

        LevelButton[] levelButtonsInScene = FindObjectsOfType<LevelButton>(true);

        // 2. Loop through each button that we found.
        foreach (LevelButton button in levelButtonsInScene)
        {
            button.gameObject.SetActive(true);

            int buttonLevelIndex = button.levelIndex;

            if (buttonLevelIndex >= 0 && buttonLevelIndex < levelManager.levels.Count)
            {
                LevelData dataForThisButton = levelManager.levels[buttonLevelIndex];
                button.Setup(dataForThisButton);
            }
            else
            {
                Debug.LogWarning($"Found a LevelButton (for index: {buttonLevelIndex}) but no matching level exists in the LevelManager. Hiding this button.", button.gameObject);
                button.gameObject.SetActive(false);
            }
        }
    }
}