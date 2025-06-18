using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class DropDownMEnu : MonoBehaviour
{
    private AudioSource audioSource;

    DropdownField disPlayResolution;
    DropdownField LanguageDropDown;

    Toggle fullScreenToggle;
    UIDocument root;
    private VisualElement settingMenu;
    string Language = "English";

    public AudioMixer audioMixer;
    Slider MusicSlider;
    Slider SFXSlider;
    private void Awake()
    {
        root = GetComponent<UIDocument>();
        audioSource = GetComponent<AudioSource>();
        settingMenu = root.rootVisualElement.Q<VisualElement>("SettingMenu");
        root.rootVisualElement.Q<Button>("CloseButton").clicked += () => StartCoroutine(HideSettings());
        root.rootVisualElement.Q<Button>("CloseButton").clicked += () => PlayClickSound();
    }
    private void Start()
    {
        // --- ADD THESE CHECKS ---
        if (audioMixer == null)
        {
            Debug.LogError("CRITICAL: AudioMixer is not assigned in the Inspector on '" + this.name + "'!", this.gameObject);
            return; // Stop the method here to prevent crashing.
        }
        if (MusicSlider == null)
        {
            Debug.LogError("CRITICAL: Could not find the UI element named 'MusicSlider'. Check the UI Document.", this.gameObject);
            return;
        }
        if (SFXSlider == null)
        {
            Debug.LogError("CRITICAL: Could not find the UI element named 'SFXSlider'. Check the UI Document.", this.gameObject);
            return;
        }

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0f); 
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);

        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("SFXVolume", sfxVolume);

        MusicSlider.value = musicVolume;
        SFXSlider.value = sfxVolume;
    }
    private void OnEnable()
    {
        InitDisplayResolution();
        InitLanguageDropDown();
        FullScreenToggle();
        ChangeVolume();
    }
    private void Update()
    {
        
    }
    public void ShowSettings()
    {
        settingMenu.style.display = DisplayStyle.Flex;
        InitDisplayResolution();
     //   InitLanguageDropDown();
    }
  

    public IEnumerator HideSettings()
    {

        yield return new WaitForSeconds(.1f);
        settingMenu.style.display = DisplayStyle.None;
    }
    private void InitDisplayResolution()
    {
        disPlayResolution = root.rootVisualElement.Q<DropdownField>("ResolutionDropDown");
      
        disPlayResolution.RegisterCallback<PointerDownEvent>(evt => PlayClickSound());

        disPlayResolution.RegisterCallback<ChangeEvent<string>>(evt => PlayClickSound());


        disPlayResolution.choices = Screen.resolutions
    .Select(resolution => $"{resolution.width}x{resolution.height}")
    .ToList();
        disPlayResolution.index = Screen.resolutions
      .Select((resolution, index) => (resolution, index))
      .First(value => value.resolution.width == Screen.currentResolution.width &&
                      value.resolution.height == Screen.currentResolution.height)
      .index;
    }
    private void InitLanguageDropDown()
    {
        LanguageDropDown = root.rootVisualElement.Q<DropdownField>("LanguageDropDown");
        LanguageDropDown.RegisterCallback<PointerDownEvent>(evt => PlayClickSound());

        LanguageDropDown.choices = new List<string> { Language};
        LanguageDropDown.index = 0;
    }

    private void FullScreenToggle()
    {
        fullScreenToggle = root.rootVisualElement.Q<Toggle>("FullScreenToggle");
        fullScreenToggle.value = Screen.fullScreen;

        fullScreenToggle.RegisterValueChangedCallback(evt =>
        {
            Screen.fullScreen = evt.newValue;
            PlayClickSound();
        });
    }
    private void PlayClickSound()
    {
        if (audioSource != null)
            audioSource.Play();
    }
    private void ChangeVolume()
    {
        MusicSlider = root.rootVisualElement.Q<Slider>("MusicSlider");
        SFXSlider = root.rootVisualElement.Q<Slider>("SFXSlider");

        MusicSlider.RegisterValueChangedCallback(evt =>
        {
            SetVolume("MusicVolume", evt.newValue);
            PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
            PlayerPrefs.Save();

        });
        SFXSlider.RegisterValueChangedCallback(evt =>
        {
            SetVolume("SFXVolume", evt.newValue);
            PlayerPrefs.SetFloat("SFXVolume", evt.newValue);
            PlayerPrefs.Save();
        });

    }
    private void SetVolume(string Name , float value)
    {
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(Name, db);
    }

}

