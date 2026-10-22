using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgMusicClip;
    public AudioClip jumpClip;
    public AudioClip hitClip;
    public AudioClip scoreClip;
    public AudioClip clickClip;

    public bool IsMusicEnabled { get; private set; } = true;
    public bool IsSFXEnabled { get; private set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup audio sources if not assigned in Inspector
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Load saved settings (defaults to enabled / 1)
        IsMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        IsSFXEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        musicSource.mute = !IsMusicEnabled;
        sfxSource.mute = !IsSFXEnabled;
    }

    private void Start()
    {
        if (bgMusicClip != null && !musicSource.isPlaying)
        {
            musicSource.clip = bgMusicClip;
            musicSource.Play();
        }
    }

    // --- SOUND EFFECTS ---

    public void PlayJump()
    {
        PlaySFX(jumpClip, 0.85f);
    }

    public void PlayHit()
    {
        PlaySFX(hitClip, 1.0f);
    }

    public void PlayScore()
    {
        PlaySFX(scoreClip, 0.9f);
    }

    public void PlayClick()
    {
        PlaySFX(clickClip, 0.8f);
    }

    private void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (!IsSFXEnabled || clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    // --- MUSIC CONTROLS ---

    public void SetMusicEnabled(bool enabled)
    {
        IsMusicEnabled = enabled;
        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (musicSource != null)
        {
            musicSource.mute = !enabled;
            if (enabled && !musicSource.isPlaying && bgMusicClip != null)
            {
                musicSource.clip = bgMusicClip;
                musicSource.Play();
            }
        }
    }

    public void SetSFXEnabled(bool enabled)
    {
        IsSFXEnabled = enabled;
        PlayerPrefs.SetInt("SFXEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (sfxSource != null)
        {
            sfxSource.mute = !enabled;
        }
    }

    public bool ToggleMusic()
    {
        SetMusicEnabled(!IsMusicEnabled);
        return IsMusicEnabled;
    }

    public bool ToggleSFX()
    {
        SetSFXEnabled(!IsSFXEnabled);
        return IsSFXEnabled;
    }
}
