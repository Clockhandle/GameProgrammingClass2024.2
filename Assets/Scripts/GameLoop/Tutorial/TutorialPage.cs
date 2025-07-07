using UnityEngine;


[CreateAssetMenu(fileName = "TutorialPage", menuName = "Tutorial/TutorialPage")]
public class TutorialPage : ScriptableObject
{
    [Tooltip("The image to display for this tutorial page.")]
    public Sprite tutorialImage;

    [Tooltip("The explanation text for this tutorial page.")]
    [TextArea(5, 10)] // Makes the text field in the Inspector larger.
    public string tutorialText;
}

