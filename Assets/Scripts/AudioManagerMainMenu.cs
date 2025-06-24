using UnityEngine;

public class AudioManagerMainMenu : MonoBehaviour
{
    public static AudioManagerMainMenu Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;

    [Header("UI Audio Clips")]
    public AudioClip uiClickClip;

    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null)
            {
                musicSource = gameObject.GetComponent<AudioSource>();
                if (musicSource == null)
                {
                    musicSource = gameObject.AddComponent<AudioSource>();
                }
                musicSource.loop = true;
                musicSource.volume = musicVolume;
            }

            if (mainMenuMusic == null)
            {
                Debug.Log("AudioManagerMainMenu: Attempting to load mainMenuMusic from Resources/Audio/Music/MainMenuMusic");
                mainMenuMusic = Resources.Load<AudioClip>("Audio/Music/MainMenuMusic");
                if (mainMenuMusic == null)
                {
                    Debug.LogWarning("AudioManagerMainMenu: Failed to load mainMenuMusic from Resources/Audio/Music/MainMenuMusic. Please add an AudioClip named 'MainMenuMusic' in that folder.");
                }
                else
                {
                    Debug.Log("AudioManagerMainMenu: Successfully loaded mainMenuMusic from Resources.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMainMenuMusic();
    }

    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null && musicSource != null)
        {
            musicSource.clip = mainMenuMusic;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManagerMainMenu: mainMenuMusic or musicSource is null.");
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
