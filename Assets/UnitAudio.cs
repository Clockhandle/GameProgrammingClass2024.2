using UnityEngine;

/// <summary>
/// Placed on individual unit prefabs to handle 3D, positional audio.
/// This script works with an AudioSource on the same GameObject and is
/// typically called by Animation Events for perfect synchronization.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class UnitAudio : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Positional Sound Clips")]
    [Tooltip("Sound for this unit's primary attack (e.g., a sword swoosh).")]
    public AudioClip attackSoundClip;

    [Tooltip("Sound for when this unit takes damage.")]
    public AudioClip hurtSoundClip;

    [Tooltip("Sound for when this unit dies.")]
    public AudioClip deathSoundClip;


    // --- Private References ---
    private AudioSource unitAudioSource;

    private void Awake()
    {
        // Get the AudioSource component that is on this same GameObject.
        // It should be configured for 3D sound (Spatial Blend = 1).
        unitAudioSource = GetComponent<AudioSource>();
    }

    // --- Public methods to be called from Animation Events ---

    /// <summary>
    /// Plays the attack sound from this unit's position.
    /// Called from an Animation Event on the attack animation.
    /// </summary>
    public void PlayAttackSound()
    {
        if (attackSoundClip != null)
        {
            // Play the clip from our own 3D audio source.
            unitAudioSource.PlayOneShot(attackSoundClip);
        }
    }

    /// <summary>
    /// Plays the footstep sound from this unit's position.
    /// Called from an Animation Event on a walk/run animation.
    /// </summary>

    // --- Public methods to be called from other scripts (like Unit.cs) ---

    /// <summary>
    /// Plays the hurt sound. Called from the Unit's TakeDamage method.
    /// </summary>
    public void PlayHurtSound()
    {
        if (hurtSoundClip != null)
        {
            unitAudioSource.PlayOneShot(hurtSoundClip);
        }
    }

    /// <summary>
    /// Plays the death sound. Called from the Unit's Die method.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSoundClip != null)
        {
            unitAudioSource.PlayOneShot(deathSoundClip);
        }
    }
}