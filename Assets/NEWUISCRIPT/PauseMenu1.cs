using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class PauseMenu1 : MonoBehaviour
{
    private VisualElement pausedMenu;
    bool isPaused;
    private AudioSource audioSource;
    public AudioClip hoverSound;
    private GameObject SettingPanel;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        SettingPanel = GameObject.Find("PauseMenu");
    }
    private void Start()
    {
       
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        pausedMenu= root.Q<VisualElement>("pauseMenu");
        root.Q<Button>("ResumeButton").clicked += ()=> ResumeGame();
        root.Q<Button>("BackToMenuButton").clicked+= ()=> StartCoroutine(BackToMenu()) ; 
        foreach (Button button in root.Query<Button>().ToList())
        {
            button.clicked += PlayClickSound;
            button.RegisterCallback<MouseEnterEvent>((evt) => PlayHoverSound());
        }
    }
    private void Update()
    {
        
    }
    public void PausedGame()
    {
        pausedMenu.style.display = DisplayStyle.Flex;

    }
    private void ResumeGame()
    {
        isPaused= false;
       // SettingPanel.SetActive(false);
        pausedMenu.style.display = DisplayStyle.None;
        GameManager.Instance.ResumeGame();
    }
    private IEnumerator BackToMenu()
    {
        
        GameManager.Instance.ResumeGame();
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.ReturnToLevelSelectorUponFailure();
        
       // Loader.Load("SampleScene");
    }
    private void PlayClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }
    private void PlayHoverSound()
    {
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
