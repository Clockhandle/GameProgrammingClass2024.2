using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Manages the state and animation of the level information panel.
/// It's designed to be controlled by UnityEvents from buttons in the scene.
/// </summary>
public class LevelInfoPanelController : MonoBehaviour
{
    // Singleton for easy access from other scripts (like LevelButton).
    public static LevelInfoPanelController Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("The main panel object that slides in and out.")]
    [SerializeField] private RectTransform panelRectTransform;

    [Tooltip("The semi-transparent backdrop.")]
    [SerializeField] private GameObject panelBackdrop;

    [Header("Panel Content")]
    [Tooltip("The TextMeshPro UI element for the level's name.")]
    [SerializeField] private TextMeshProUGUI levelNameText;

    [Tooltip("The TextMeshPro UI element for the level's description.")]
    [SerializeField] private TextMeshProUGUI levelDescriptionText;

    [Tooltip("The Image component on the map preview button.")]
    [SerializeField] private Image miniMapPreviewImage;
    [SerializeField] private Image largeMapPreviewImage;

    [Header("Animation Settings")]
    [Tooltip("How long the slide animation should take in seconds.")]
    [SerializeField] private float animationSpeed = 0.3f;

    // We still need to store the level data to pass it to other buttons/systems.
    public LevelData CurrentLevelData { get; private set; }

    private Vector2 panelOnscreenPosition;
    private Vector2 panelOffscreenPosition;
    private bool isPanelVisible = false;

    private void Awake()
    {
        // Set up the singleton instance.
        Instance = this;
    }

    private void Start()
    {
        // Calculate the on-screen and off-screen positions based on the panel's width.
        // This assumes the panel is anchored to the right edge of the screen.
        panelOnscreenPosition = Vector2.zero; // Anchored position (0,0) is centered on the anchor.
        float panelWidth = panelRectTransform.rect.width;
        panelOffscreenPosition = new Vector2(panelWidth, 0);

        // Start with the panel hidden and off-screen.
        panelRectTransform.anchoredPosition = panelOffscreenPosition;
        panelBackdrop.SetActive(false);
    }

    /// <summary>
    /// This is the main function to be called by your Level Button.
    /// It receives the level's data, populates the UI, and starts the animation.
    /// </summary>
    /// <param name="data">The ScriptableObject containing the level's information.</param>
    public void PrepareAndShowPanel(LevelData data)
    {
        if (isPanelVisible || data == null) return;

        isPanelVisible = true;

        // 1. Store the data for other components to use.
        CurrentLevelData = data;

        // 2. Populate the UI elements with the new data.
        levelNameText.text = data.levelName;
        // You'll need to add a description field to your LevelData scriptable object.
        levelDescriptionText.text = data.levelDescription; 

        if (data.levelIcon != null)
        {
            miniMapPreviewImage.sprite = data.levelIcon;
            largeMapPreviewImage.sprite = data.levelIcon;
        }

        // 3. Show the backdrop and begin the slide-in animation.
        panelBackdrop.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimatePanel(panelRectTransform.anchoredPosition, panelOnscreenPosition));
    }

    /// <summary>
    /// This public function should be assigned to the 'OnClick' event of your backdrop button.
    /// </summary>
    public void HidePanel()
    {
        if (!isPanelVisible) return;

        isPanelVisible = false;

        // Begin the slide-out animation.
        StopAllCoroutines();
        StartCoroutine(AnimatePanel(panelRectTransform.anchoredPosition, panelOffscreenPosition));

        // Hide the backdrop after the animation is complete.
        StartCoroutine(DeactivateBackdropAfterDelay(animationSpeed));
    }

    /// <summary>
    /// The coroutine that handles the smooth sliding animation of the panel.
    /// </summary>
    private IEnumerator AnimatePanel(Vector2 startPos, Vector2 endPos)
    {
        float timer = 0f;
        while (timer < animationSpeed)
        {
            timer += Time.deltaTime;
            // Calculate the progress from 0 to 1.
            float progress = Mathf.Clamp01(timer / animationSpeed);
            // Use Lerp to move the panel smoothly.
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
            yield return null; // Wait for the next frame.
        }
        // Ensure the panel is exactly at its final destination.
        panelRectTransform.anchoredPosition = endPos;
    }

    /// <summary>
    /// A simple coroutine to wait for the animation to finish before hiding the backdrop.
    /// </summary>
    private IEnumerator DeactivateBackdropAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        panelBackdrop.SetActive(false);
    }
}