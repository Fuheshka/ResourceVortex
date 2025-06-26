using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource musicSourceA;


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

    [Header("Trash Bin Audio Clips")]
    public AudioClip trashBinClearClip; // New clip for trash bin clear sound

    [Header("Enemy Audio Clips")]
    public AudioClip enemySpawnClip;
    public AudioClip[] enemyDeathClips;

    [Header("Gameplay Audio Clips")]
    public AudioClip scoreClip;
    public AudioClip[] trashRecycleClips; // Changed to array for random recycle sounds
    public AudioClip upgradePurchaseClip;

    [Header("UI Audio Clips")]
    public AudioClip uiClickClip;

    [Header("Music Clips")]
    public AudioClip calmMusic;
    public AudioClip dynamicMusic;
    public AudioClip mainMenuMusic; // Added main menu music clip

    public float fadeDuration = 2f;

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private WaveManager waveManager;
    private bool isDynamicMusicPlaying = false;
    private bool isMusicLocked = false;

    public string gameSceneName = "SampleScene";



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure musicSourceA AudioSource exists
            if (musicSourceA == null)
            {
                musicSourceA = gameObject.AddComponent<AudioSource>();
                musicSourceA.loop = true;
                musicSourceA.volume = musicVolume;
            }

            // Load mainMenuMusic from Resources if not assigned
            if (mainMenuMusic == null)
            {
                Debug.Log("AudioManager: Attempting to load mainMenuMusic from Resources/Audio/Music/MainMenuMusic");
                mainMenuMusic = Resources.Load<AudioClip>("Audio/Music/MainMenuMusic");
                if (mainMenuMusic == null)
                {
                    Debug.LogWarning("AudioManager: Failed to load mainMenuMusic from Resources/Audio/Music/MainMenuMusic. Please add an AudioClip named 'MainMenuMusic' in that folder.");
                }
                else
                {
                    Debug.Log("AudioManager: Successfully loaded mainMenuMusic from Resources.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameSceneName)
        {
            Debug.Log("AudioManager: Game scene loaded, resetting music state.");
            ResetMusicState();
        }
        else if (scene.name == "MainMenu")
        {
            Debug.Log("AudioManager: Main menu scene loaded, destroying game AudioManager.");
            Destroy(gameObject);
            Instance = null;
        }
    }

public void ResetMusicState()
{
    Debug.Log("AudioManager: ResetMusicState called, switching to calmMusic.");

    if (musicSourceA != null && calmMusic != null)
    {
        PlayMusic(calmMusic);
        Debug.Log("AudioManager: calmMusic started playing.");
    }
    else
    {
        Debug.LogWarning("AudioManager: musicSourceA or calmMusic is null in ResetMusicState.");
    }
}

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();

        if (waveManager == null)
        {
            Debug.LogError("WaveManager not found in scene.");
        }

        ResetMusicState();
    }

private void Update()
{
    if (waveManager == null || musicSourceA == null) return;

    bool enemiesPresent = waveManager.IsWaveInProgress() || (FindObjectsOfType<EnemyAI>().Length > 0);

    if (enemiesPresent && !isDynamicMusicPlaying)
    {
        PlayMusic(dynamicMusic);
        isDynamicMusicPlaying = true;
    }
    else if (!enemiesPresent && isDynamicMusicPlaying)
    {
        PlayMusic(calmMusic);
        isDynamicMusicPlaying = false;
    }
}

public void PlaySFX(AudioClip clip)
{
    if (clip != null && sfxSource != null)
    {
        Debug.Log($"AudioManager: Playing SFX clip {clip.name}");
        sfxSource.PlayOneShot(clip);
    }
    else
    {
        Debug.LogWarning("AudioManager: PlaySFX called with null clip or sfxSource");
    }
}

    public void PlayEnemyDeathSFX()
    {
        if (enemyDeathClips != null && enemyDeathClips.Length > 0)
        {
            int index = Random.Range(0, enemyDeathClips.Length);
            PlaySFX(enemyDeathClips[index]);
        }
    }

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

    public void PlayTrashBinClearSFX()
    {
        if (trashBinClearClip != null && recycleSfxSource != null)
        {
            if (recycleSfxSource.isPlaying)
            {
                recycleSfxSource.Stop();
            }
            recycleSfxSource.clip = trashBinClearClip;
            recycleSfxSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: PlayTrashBinClearSFX called but trashBinClearClip or recycleSfxSource is null.");
        }
    }

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

    public void StartRunSFX()
    {
        if (runClips != null && runClips.Length > 0 && runSfxSource != null && !runSfxSource.isPlaying)
        {
            PlayRunSFX();
        }
    }

    public void StopRunSFX()
    {
        if (runSfxSource != null && runSfxSource.isPlaying)
        {
            runSfxSource.Stop();
        }
    }

public void PlayMusic(AudioClip clip)
{
    if (clip != null && musicSourceA != null)
    {
        if (musicSourceA.isPlaying)
        {
            musicSourceA.Stop();
        }
        Debug.Log($"AudioManager: Playing music clip {clip.name}");
        musicSourceA.clip = clip;
        musicSourceA.volume = musicVolume;
        musicSourceA.loop = true;
        musicSourceA.Play();
    }
    else
    {
        Debug.LogWarning("AudioManager: PlayMusic called with null clip or musicSourceA");
    }
}

public void PlayMainMenuMusic()
{
    if (mainMenuMusic != null)
    {
        PlayMusic(mainMenuMusic);
    }
    else
    {
        Debug.LogWarning("AudioManager: mainMenuMusic clip is not assigned.");
    }
}


public void SetMusicVolume(float volume)
{
    musicVolume = Mathf.Clamp01(volume);
    Debug.Log($"AudioManager: SetMusicVolume called with volume {musicVolume}");
    if (musicSourceA != null)
    {
        musicSourceA.volume = musicVolume;
        Debug.Log($"AudioManager: musicSourceA volume set to {musicSourceA.volume}");
    }
}

    public float GetMusicVolume()
    {
        if (musicSourceA != null)
        {
            return musicSourceA.volume;
        }
        return musicVolume;
    }

    public bool IsMusicPlaying()
    {
        if (musicSourceA != null)
        {
            return musicSourceA.isPlaying;
        }
        return false;
    }

    public IEnumerator FadeOutMusic(float duration)
    {
        if (musicSourceA == null) yield break;

        float startVolume = musicSourceA.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicSourceA.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        musicSourceA.Stop();
        musicSourceA.volume = musicVolume;
    }

    public IEnumerator FadeInMusic(AudioClip clip, float duration)
    {
        if (musicSourceA == null || clip == null) yield break;

        musicSourceA.clip = clip;
        musicSourceA.volume = 0f;
        musicSourceA.loop = true;
        musicSourceA.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicSourceA.volume = Mathf.Lerp(0f, musicVolume, time / duration);
            yield return null;
        }

        musicSourceA.volume = musicVolume;
    }

}
