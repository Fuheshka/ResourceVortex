using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioSource recycleSfxSource; // New AudioSource for recycle sound
    public AudioSource walkSfxSource; // New AudioSource for walk sound loop

    [Header("Player Audio Clips")]
    public AudioClip playerThrowClip;
    public AudioClip jumpClip;
    public AudioClip walkClip;
    public AudioClip shieldActivateClip;

    [Header("Enemy Audio Clips")]
    public AudioClip enemySpawnClip;
    public AudioClip enemyDeathClip;

    [Header("Gameplay Audio Clips")]
    public AudioClip scoreClip;
    public AudioClip[] trashRecycleClips; // Changed to array for random recycle sounds
    public AudioClip upgradePurchaseClip;

    [Header("UI Audio Clips")]
    public AudioClip uiClickClip;
    public AudioClip backgroundMusicClip;

    private void Awake()
    {
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

    private void Start()
    {
        PlayMusic(backgroundMusicClip);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // New method to play recycle sound, interrupting previous recycle sound if playing
    public void PlayRecycleSFX(AudioClip clip)
    {
        if (clip != null && recycleSfxSource != null)
        {
            if (recycleSfxSource.isPlaying)
            {
                recycleSfxSource.Stop();
            }
            recycleSfxSource.clip = clip;
            recycleSfxSource.Play();
        }
    }

    // New methods to control walk sound loop
    public void StartWalkSFX()
    {
        if (walkClip != null && walkSfxSource != null && !walkSfxSource.isPlaying)
        {
            walkSfxSource.clip = walkClip;
            walkSfxSource.loop = true;
            walkSfxSource.Play();
        }
    }

    public void StopWalkSFX()
    {
        if (walkSfxSource != null && walkSfxSource.isPlaying)
        {
            walkSfxSource.Stop();
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
