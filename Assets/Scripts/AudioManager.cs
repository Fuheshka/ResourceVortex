using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

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
    public AudioClip trashRecycleClip;
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
