using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class WaveData
{
    public string waveName; // Name of the wave for display
    public List<EnemySpawnConfig> enemies; // Enemies to spawn
    public float spawnInterval = 3f; // Interval between spawns
    public float delayBeforeWave = 5f; // Delay before wave starts
}

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab; // Enemy prefab
    public int count; // Number of enemies
}

[System.Serializable]
public class Wave
{
    public string waveName; // Name of the wave for display
    public List<EnemySpawnConfig> enemies; // Enemies to spawn
    public float spawnInterval = 3f; // Interval between spawns
    public float delayBeforeWave = 5f; // Delay before wave starts
}

public class WaveManager : MonoBehaviour
{
    public List<Wave> waves; // List of waves
    public List<EnemySpawner> spawners; // Enemy spawners
    public TextMeshProUGUI waveText; // UI text for wave display
    public Button continueButton; // Continue button to proceed after upgrades

    private int currentWaveIndex = 0;
    private bool isWaveInProgress = false;
    private bool waitingForContinue = false;

    public UpgradeSystem upgradeSystem; // Reference to UpgradeSystem

    void Awake()
    {
        // No singleton or DontDestroyOnLoad
    }

    private void OnDestroy()
    {
        // No sceneLoaded event unsubscription needed
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Remove this method or adjust if subscribing to sceneLoaded
    }

    private void ResetInternalState()
    {
        // Add any additional reset logic here if needed
        Debug.Log("WaveManager: ResetInternalState called");
    }

    public void ResetWaves()
    {
        Debug.Log("WaveManager: ResetWaves called");
        currentWaveIndex = 0;
        isWaveInProgress = false;
    }

    public void RestartWaves()
    {
        Debug.Log("WaveManager: RestartWaves called");
        StartCoroutine(StartWaves());
    }

    public void ResetAndStartWaves()
    {
        Debug.Log("WaveManager: ResetAndStartWaves called");
        StopAllCoroutines();
        ResetWaves();
        RestartWaves();
    }

    void Start()
    {
        // Initialize continue button listener
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }

        // ������������� UI ������
        if (waveText != null)
        {
            UpdateWaveText();
        }

        // ������� ��� ��������, ���� �� ��������� �������
        if (spawners.Count == 0)
        {
            spawners.AddRange(FindObjectsOfType<EnemySpawner>());
        }

        // Removed automatic start of waves on Start
        // StartCoroutine(StartWaves());
    }

    public void StartWaveSequence()
    {
        StartCoroutine(StartWaves());
    }

    public void ReinitializeReferences()
    {
        Debug.Log("WaveManager: ReinitializeReferences called");
        // Always find and assign spawners and waveText after scene reload
        spawners = new List<EnemySpawner>(FindObjectsOfType<EnemySpawner>());

        GameObject waveTextObj = GameObject.Find("WaveNumber");
        if (waveTextObj != null)
        {
            waveText = waveTextObj.GetComponent<TMPro.TextMeshProUGUI>();
        }
        else
        {
            waveText = FindObjectOfType<TMPro.TextMeshProUGUI>();
        }
    }

    IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            isWaveInProgress = false;

            // Delay before wave with countdown display
            float delay = currentWave.delayBeforeWave;
            while (delay > 0f)
            {
                if (waveText != null)
                {
                    waveText.text = $"Next Wave ({currentWaveIndex + 1}/{waves.Count}): {currentWave.waveName} starts in {delay:F1} seconds";
                }
                yield return null;
                delay -= Time.deltaTime;
            }

            // Wave started
            isWaveInProgress = true;
            if (waveText != null)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Count}: {currentWave.waveName}";
            }
            Debug.Log($"Starting Wave {currentWaveIndex + 1}: {currentWave.waveName}");

            // Spawn enemies
            int totalEnemies = 0;
            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                totalEnemies += config.count;
            }

            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                int enemiesPerSpawner = config.count / spawners.Count;
                int extraEnemies = config.count % spawners.Count; // remainder for first spawner

                for (int i = 0; i < spawners.Count; i++)
                {
                    int count = enemiesPerSpawner + (i == 0 ? extraEnemies : 0);
                    if (count > 0)
                    {
                        spawners[i].QueueEnemies(config.enemyPrefab, count, currentWave.spawnInterval);
                    }
                }
            }

            // Wait for all enemies to be defeated and spawning to finish
            while (GameObject.FindObjectsOfType<EnemyAI>().Length > 0 || spawners.Exists(s => s.IsSpawning()))
            {
                yield return null;
            }

            isWaveInProgress = false;
            currentWaveIndex++;
            UpdateWaveText();

                // Convert score to upgrade currency after each wave
                if (upgradeSystem != null)
                {
                    upgradeSystem.ConvertScoreToCurrency();

                    // Do not pause game for upgrades, just show upgrade UI and wait for player to continue
                    UpgradeUI upgradeUI = upgradeSystem.GetComponent<UpgradeUI>();
                    if (upgradeUI != null)
                    {
                        upgradeUI.UpdateConvertedCurrencyText();
                        upgradeUI.gameObject.SetActive(true);
                    }
                }

                if (currentWaveIndex >= waves.Count)
                {
                    Debug.Log("All waves completed!");
                    if (waveText != null)
                    {
                        waveText.text = "All Waves Completed!";
                    }
                    break;
                }
            }
        }

    void UpdateWaveText()
    {
        if (waveText != null)
        {
            if (currentWaveIndex < waves.Count)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Count}";
            }
        }
    }

    public bool IsWaveInProgress()
    {
        return isWaveInProgress;
    }

        // Method to be called by UI button to continue game after upgrades
        public void ContinueGame()
        {
                // Hide upgrade UI
                if (upgradeSystem != null && upgradeSystem.GetComponent<UpgradeUI>() != null)
                {
                    upgradeSystem.GetComponent<UpgradeUI>().gameObject.SetActive(false);
                }

                // Start next wave if any
                if (!isWaveInProgress && currentWaveIndex < waves.Count)
                {
                    StartCoroutine(StartWaves());
                }
            }
}
