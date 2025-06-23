using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    public AudioSource recycleSfxSource; // New AudioSource for recycle sound
    public AudioSource walkSfxSource; // New AudioSource for walk sound loop
    public AudioSource runSfxSource; // New AudioSource for run sound sequence

    [Header("Player Audio Clips")]
    public AudioClip playerThrowClip;
    public AudioClip[] jumpStartClips; // New array for jump start sounds
    public AudioClip[] jumpLandClips; // New array for landing sounds
    public AudioClip[] walkClips; // Changed to array for random walk sounds
    public AudioClip[] runClips; // New array for random run sounds
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
        if (walkClips != null && walkClips.Length > 0 && walkSfxSource != null && !walkSfxSource.isPlaying)
        {
            int index = Random.Range(0, walkClips.Length);
            walkSfxSource.clip = walkClips[index];
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

    // New method to play random run footstep sound without overlapping
    public void PlayRunSFX()
    {
        if (runClips != null && runClips.Length > 0 && runSfxSource != null)
        {
            if (!runSfxSource.isPlaying)
            {
                int index = Random.Range(0, runClips.Length);
                runSfxSource.clip = runClips[index];
                runSfxSource.Play();
            }
        }
    }

    // New method to start running footstep sequence
    public void StartRunSFX()
    {
        if (runClips != null && runClips.Length > 0 && runSfxSource != null && !runSfxSource.isPlaying)
        {
            PlayRunSFX();
        }
    }

    // New method to stop running footstep sequence
    public void StopRunSFX()
    {
        if (runSfxSource != null && runSfxSource.isPlaying)
        {
            runSfxSource.Stop();
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
