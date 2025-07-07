using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutorialPanelController : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("The tutorial asset to be displayed by this panel.")]
    public Tutorial tutorial;

    [Header("UI Elements")]
    public Image displayImage;
    public TextMeshProUGUI displayText;
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    private int currentPageIndex = 0;

    public static bool IsTutorialOpen { get; private set; }

    void Start()
    {
        ShowPage(currentPageIndex);

        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(CloseTutorial);
    }

    private void ShowPage(int index)
    {
        TutorialPage currentPage = tutorial.pages[index];

        displayImage.sprite = currentPage.tutorialImage;
        displayText.text = currentPage.tutorialText;

        UpdateNavButtons();
    }

    private void NextPage()
    {
        if (currentPageIndex < tutorial.pages.Count - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }
    }

    private void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }
    }

    private void UpdateNavButtons()
    {
        prevButton.interactable = (currentPageIndex > 0);
        nextButton.interactable = (currentPageIndex < tutorial.pages.Count - 1);
    }

    private void CloseTutorial()
    {
        IsTutorialOpen = false;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenTutorial()
    {
        IsTutorialOpen = true; // Set the flag
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
    }

}
