using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("The name of the level that will be displayed in the UI.")]
    public string levelName;

    [Tooltip("The name of the scene to load for this level.")]
    public string sceneToLoad;

    [Tooltip("The name of the character selection scene to load.")]
    public string charSceneToLoad;

    [Tooltip("A brief description of the level or its objectives.")]
    [TextArea]
    public string levelDescription;

    [Tooltip("A sprite or image representing the level.")]
    public Sprite levelIcon;

    [Tooltip("Is this level currently unlocked?")]
    public bool isUnlocked;

    [Tooltip("The background music to play during this level.")]
    public AudioClip levelMusic;
}