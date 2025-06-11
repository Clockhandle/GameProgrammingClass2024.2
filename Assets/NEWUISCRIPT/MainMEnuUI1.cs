using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMEnuUI1 : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip hoverSound;

    public DropDownMEnu settingsUI;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        root.Q<Button>("StartButton").clicked+= () => StartCoroutine(StartGame());
        root.Q<Button>("SettingButton").clicked+= () => StartCoroutine(SettingGame());
        root.Q<Button>("QuitButton").clicked+= ()=>QuitGame();
        foreach (Button button in root.Query<Button>().ToList())
        {
            button.clicked += PlayClickSound;
            // Play hover sound
            button.RegisterCallback<MouseEnterEvent>((evt) => PlayHoverSound());
        }

      


    }
  

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.3f);
        Loader.Load("LevelSelector");
    }
    private IEnumerator SettingGame()
    {
        yield return new WaitForSeconds(0.15f);
        settingsUI.ShowSettings();
    }
    private void QuitGame()
    {
        Application.Quit();
    }
    private void OnDisable()
    {
        
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
