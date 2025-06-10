using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class PauseMenu1 : MonoBehaviour
{
    private VisualElement pausedMenu;
    bool isPaused;
    private AudioSource audioSource;
    public AudioClip hoverSound;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PausedGame();
            }
        }
    }
    private void PausedGame()
    {
        isPaused = true;
        pausedMenu.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;

    }
    private void ResumeGame()
    {
        isPaused= false;    
        pausedMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
    private IEnumerator BackToMenu()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(0.3f);
        Loader.Load("SampleScene");
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
