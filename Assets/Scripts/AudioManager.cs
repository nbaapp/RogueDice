using UnityEngine;

/// <summary>
/// Simple audio manager for playing game sound effects.
/// Handles dice roll sounds and other game audio.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("AudioSource component for playing sound effects.")]
    public AudioSource sfxAudioSource;
    
    [Header("Sound Effects")]
    [Tooltip("Sound effect played when dice are rolled or rerolled.")]
    public AudioClip diceRollSFX;
    
    [Tooltip("Sound effect played when cards are dealt (future use).")]
    public AudioClip cardDealSFX;
    
    [Header("Audio Settings")]
    [Tooltip("Volume for sound effects (0.0 to 1.0).")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    
    [Tooltip("Whether sound effects are enabled.")]
    public bool sfxEnabled = true;
    
    // Singleton pattern for easy access
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Initialize audio source if not assigned
        if (sfxAudioSource == null)
        {
            sfxAudioSource = GetComponent<AudioSource>();
            if (sfxAudioSource == null)
            {
                // Create an AudioSource component if none exists
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Configure audio source
        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
            sfxAudioSource.volume = sfxVolume;
        }
    }
    
    /// <summary>
    /// Plays the dice roll sound effect.
    /// </summary>
    public void PlayDiceRollSound()
    {
        PlaySFX(diceRollSFX, "Dice Roll");
    }
    
    /// <summary>
    /// Plays the card deal sound effect.
    /// </summary>
    public void PlayCardDealSound()
    {
        PlaySFX(cardDealSFX, "Card Deal");
    }
    
    /// <summary>
    /// Plays a specific sound effect.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="soundName">Name of the sound for debugging.</param>
    public void PlaySFX(AudioClip clip, string soundName = "SFX")
    {
        if (!sfxEnabled)
        {
            Debug.Log($"[AudioManager] SFX disabled, skipping {soundName}");
            return;
        }
        
        if (sfxAudioSource == null)
        {
            Debug.LogWarning($"[AudioManager] Cannot play {soundName} - AudioSource is null!");
            return;
        }
        
        if (clip == null)
        {
            Debug.LogWarning($"[AudioManager] Cannot play {soundName} - AudioClip is null!");
            return;
        }
        
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.PlayOneShot(clip);
        Debug.Log($"[AudioManager] Playing {soundName} sound effect");
    }
    
    /// <summary>
    /// Sets the SFX volume.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0).</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = sfxVolume;
        }
        Debug.Log($"[AudioManager] SFX volume set to {sfxVolume}");
    }
    
    /// <summary>
    /// Enables or disables sound effects.
    /// </summary>
    /// <param name="enabled">Whether SFX should be enabled.</param>
    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        Debug.Log($"[AudioManager] SFX {(enabled ? "enabled" : "disabled")}");
    }
}
