using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// A helper class to make organizing sounds easier in the Inspector.
/// </summary>
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

/// <summary>
/// A persistent singleton manager for handling global 2D audio like music and UI sounds.
/// This object should live in the first scene and not be destroyed on load.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("The AudioSource for playing background music. Should be set to loop.")]
    [SerializeField] private AudioSource musicSource;

    [Tooltip("The AudioSource for playing one-shot 2D sound effects.")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Library")]
    [Tooltip("A list of all global sound effects used in the game (e.g., UI clicks).")]
    [SerializeField] private List<Sound> sfxList;

    private Coroutine musicLoopCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Plays a non-positional (2D) sound effect by its name.
    /// Perfect for UI clicks or other global notifications.
    /// </summary>
    /// <param name="name">The name of the sound from the SFX List.</param>
    public void PlaySFX(string name)
    {
        Sound soundToPlay = sfxList.Find(s => s.name == name);

        if (soundToPlay != null)
        {
            sfxSource.PlayOneShot(soundToPlay.clip);
        }
        else
        {
            Debug.LogWarning($"SoundManager: SFX with name '{name}' not found in the library!");
        }
    }

    /// <summary>
    /// Changes the looping background music track.
    /// </summary>
    /// <param name="newMusic">The new music clip to play.</param>
    public void ChangeMusic(AudioClip newMusic)
    {
        if (musicLoopCoroutine != null)
        {
            StopCoroutine(musicLoopCoroutine);
            musicLoopCoroutine = null;
        }

        if (musicSource.clip == newMusic) return;

        musicSource.clip = newMusic;
        musicSource.loop = true; // Use standard looping for this method.
        musicSource.Play();
    }

    /// <summary>
    /// Plays a music track with a custom intro and loop section.
    /// </summary>
    /// <param name="musicWithIntro">The full audio clip.</param>
    /// <param name="loopStartTime">The time in seconds where the loop should begin.</param>
    public void ChangeMusicWithIntro(AudioClip musicWithIntro, float loopStartTime)
    {
        if (musicLoopCoroutine != null)
        {
            StopCoroutine(musicLoopCoroutine);
        }

        // Start the new music loop coroutine.
        musicLoopCoroutine = StartCoroutine(MusicLoopCoroutine(musicWithIntro, loopStartTime));
    }

    /// <summary>
    /// A coroutine that manages playing a track with an intro, then looping a specific section.
    /// </summary>
    private IEnumerator MusicLoopCoroutine(AudioClip clip, float loopStart)
    {
        musicSource.clip = clip;
        musicSource.loop = false;

        musicSource.Play();

        yield return new WaitUntil(() => musicSource.time >= loopStart);

        while (true)
        {
            if (!musicSource.isPlaying || musicSource.time >= musicSource.clip.length - 0.1f)
            {
                musicSource.time = loopStart;
                musicSource.Play();
            }

            yield return null;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}