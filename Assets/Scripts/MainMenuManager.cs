using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "SampleScene";

    private AudioManagerMainMenu audioManagerMainMenu;

    private void Start()
    {
        if (AudioManagerMainMenu.Instance == null)
        {
            GameObject audioManagerObject = new GameObject("AudioManagerMainMenu");
            audioManagerMainMenu = audioManagerObject.AddComponent<AudioManagerMainMenu>();
        }
        else
        {
            audioManagerMainMenu = AudioManagerMainMenu.Instance;
        }

        if (audioManagerMainMenu != null)
        {
            audioManagerMainMenu.PlayMainMenuMusic();
        }
    }

    public void StartGame()
    {
        // Play UI click sound
        if (audioManagerMainMenu != null)
        {
            audioManagerMainMenu.PlaySFX(audioManagerMainMenu.uiClickClip);
        }

        // Destroy the main menu audio manager before loading game scene
        if (audioManagerMainMenu != null)
        {
            Destroy(audioManagerMainMenu.gameObject);
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game");

        // Play UI click sound
        if (audioManagerMainMenu != null)
        {
            audioManagerMainMenu.PlaySFX(audioManagerMainMenu.uiClickClip);
        }

        Application.Quit();
    }
}
