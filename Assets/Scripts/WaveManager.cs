using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Wave
{
    public string waveName; // �������� ����� ��� �������� � ����������
    public List<EnemySpawnConfig> enemies; // ������ ������ ��� ������
    public float spawnInterval = 3f; // �������� ����� �������� ������ � �����
    public float delayBeforeWave = 5f; // �������� ����� ������� �����
}

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab; // ������ �����
    public int count; // ���������� ������ ����� ����
}


public class WaveManager : MonoBehaviour
{
    public List<Wave> waves; // List of waves
    public List<EnemySpawner> spawners; // Enemy spawners
    public TextMeshProUGUI waveText; // UI text for wave display
    private int currentWaveIndex = 0;
    private bool isWaveInProgress = false;

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

        // ��������� ������ �����
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
            isWaveInProgress = true;

            // ��������� UI
            if (waveText != null)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}: {currentWave.waveName}";
            }
            Debug.Log($"Starting Wave {currentWaveIndex + 1}: {currentWave.waveName}");

            // ���� ����� ������� �����
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            // ������������ ������ �� ���������
            int totalEnemies = 0;
            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                totalEnemies += config.count;
            }

            // ������������ ������ ���������� ����� ����������
            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                int enemiesPerSpawner = config.count / spawners.Count;
                int extraEnemies = config.count % spawners.Count; // ������� ��� ������� ��������

                for (int i = 0; i < spawners.Count; i++)
                {
                    int count = enemiesPerSpawner + (i == 0 ? extraEnemies : 0);
                    if (count > 0)
                    {
                        spawners[i].QueueEnemies(config.enemyPrefab, count, currentWave.spawnInterval);
                    }
                }
            }

            // ����, ���� ��� ����� � ����� ����� ���������� � ����������
            while (GameObject.FindObjectsOfType<EnemyAI>().Length > 0 || spawners.Exists(s => s.IsSpawning()))
            {
                yield return null;
            }

            isWaveInProgress = false;
            currentWaveIndex++;
            UpdateWaveText();

            // ���� ����� �����������
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
}