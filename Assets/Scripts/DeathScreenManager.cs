using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject deathScreenUI;
    public Button restartButton;
    public Button quitButton;
    public TextMeshProUGUI bestScoreText;

    [Header("Player Controller")]
    public MonoBehaviour playerController; // Reference to player controller script to disable on death

    private bool isDeathScreenActive = false;

    void Awake()
    {
        // No singleton or DontDestroyOnLoad
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        if (deathScreenUI == null)
        {
            deathScreenUI = GameObject.Find("DeathScreenUI");
        }

        if (restartButton == null)
        {
            GameObject restartObj = GameObject.Find("RestartButton");
            if (restartObj != null)
            {
                restartButton = restartObj.GetComponent<Button>();
                if (restartButton != null)
                {
                    restartButton.onClick.RemoveAllListeners();
                    restartButton.onClick.AddListener(RestartGame);
                }
            }
        }

        if (quitButton == null)
        {
            GameObject quitObj = GameObject.Find("QuitButton");
            if (quitObj != null)
            {
                quitButton = quitObj.GetComponent<Button>();
                if (quitButton != null)
                {
                    quitButton.onClick.RemoveAllListeners();
                    quitButton.onClick.AddListener(QuitGame);
                }
            }
        }

        if (bestScoreText == null)
        {
            GameObject bestScoreObj = GameObject.Find("BestScoreText");
            if (bestScoreObj != null)
            {
                bestScoreText = bestScoreObj.GetComponent<TMPro.TextMeshProUGUI>();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method can be removed or adjusted since no longer subscribing to sceneLoaded
    }

    private void InitializeUIReferences()
    {
        if (deathScreenUI == null)
        {
            deathScreenUI = GameObject.Find("DeathScreenUI");
            if (deathScreenUI == null)
            {
                Debug.LogWarning("DeathScreenManager: Could not find DeathScreenUI GameObject");
            }
        }

        if (restartButton == null)
        {
            GameObject restartObj = GameObject.Find("RestartButton");
            if (restartObj != null)
            {
                restartButton = restartObj.GetComponent<Button>();
                if (restartButton != null)
                {
                    restartButton.onClick.RemoveAllListeners();
                    restartButton.onClick.AddListener(RestartGame);
                }
                else
                {
                    Debug.LogWarning("DeathScreenManager: RestartButton component missing on GameObject");
                }
            }
            else
            {
                Debug.LogWarning("DeathScreenManager: Could not find RestartButton GameObject");
            }
        }

        if (quitButton == null)
        {
            GameObject quitObj = GameObject.Find("QuitButton");
            if (quitObj != null)
            {
                quitButton = quitObj.GetComponent<Button>();
                if (quitButton != null)
                {
                    quitButton.onClick.RemoveAllListeners();
                    quitButton.onClick.AddListener(QuitGame);
                }
                else
                {
                    Debug.LogWarning("DeathScreenManager: QuitButton component missing on GameObject");
                }
            }
            else
            {
                Debug.LogWarning("DeathScreenManager: Could not find QuitButton GameObject");
            }
        }

        if (bestScoreText == null)
        {
            GameObject bestScoreObj = GameObject.Find("BestScoreText");
            if (bestScoreObj != null)
            {
                bestScoreText = bestScoreObj.GetComponent<TMPro.TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("DeathScreenManager: Could not find BestScoreText GameObject");
            }
        }
    }

    private void ResetDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }
        isDeathScreenActive = false;
    }

    void Start()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void ShowDeathScreen()
    {
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        isDeathScreenActive = true;

        UpdateBestScore();

        // Pause the game
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        if (isDeathScreenActive)
        {
            ResetDeathScreen();
        }
        StartCoroutine(RestartSceneCoroutine());
    }

    private System.Collections.IEnumerator RestartSceneCoroutine()
    {
        // Resume the game
        Time.timeScale = 1f;

        // Wait for end of frame to ensure proper reset
        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Wait for scene to load
        yield return new WaitForSeconds(0.1f);

        // Ensure time scale is reset after scene load
        Time.timeScale = 1f;

        // Find EnemySpawner and enable it explicitly
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.enabled = true;
            Debug.Log("EnemySpawner enabled after scene reload");
        }

        // Reset WaveManager state and restart waves
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            Debug.Log("Resetting WaveManager state and restarting waves");
            waveManager.ReinitializeReferences();
            waveManager.StopAllCoroutines();
            waveManager.ResetWaves();
            waveManager.RestartWaves();
        }
    }

    void UpdateBestScore()
    {
        if (ScoreManager.Instance == null || bestScoreText == null)
        {
            return;
        }

        int currentScore = ScoreManager.Instance.GetScore();
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        bestScoreText.text = $"Best Score: {bestScore}";
    }

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    public void QuitGame()
    {
        // Load main menu scene instead of quitting application
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            Time.timeScale = 1f; // Resume time scale before loading scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Application.Quit();
        }
    }
}
