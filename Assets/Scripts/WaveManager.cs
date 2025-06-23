using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName; // Название волны для удобства в инспекторе
    public List<EnemySpawnConfig> enemies; // Список врагов для спавна
    public float spawnInterval = 3f; // Интервал между спавнами врагов в волне
    public float delayBeforeWave = 5f; // Задержка перед началом волны
}

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab; // Префаб врага
    public int count; // Количество врагов этого типа
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance; // Singleton для доступа из других скриптов
    public List<Wave> waves; // Список всех волн
    public List<EnemySpawner> spawners; // Список спавнеров
    public TextMeshProUGUI waveText; // UI текст для отображения текущей волны (опционально)
    private int currentWaveIndex = 0;
    private bool isWaveInProgress = false;

    void Awake()
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

    void Start()
    {
        // Инициализация UI текста
        if (waveText != null)
        {
            UpdateWaveText();
        }

        // Находим все спавнеры, если не назначены вручную
        if (spawners.Count == 0)
        {
            spawners.AddRange(FindObjectsOfType<EnemySpawner>());
        }

        // Запускаем первую волну
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave currentWave = waves[currentWaveIndex];
            isWaveInProgress = true;

            // Обновляем UI
            if (waveText != null)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}: {currentWave.waveName}";
            }
            Debug.Log($"Starting Wave {currentWaveIndex + 1}: {currentWave.waveName}");

            // Ждем перед началом волны
            yield return new WaitForSeconds(currentWave.delayBeforeWave);

            // Распределяем врагов по спавнерам
            int totalEnemies = 0;
            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                totalEnemies += config.count;
            }

            // Распределяем врагов равномерно между спавнерами
            foreach (EnemySpawnConfig config in currentWave.enemies)
            {
                int enemiesPerSpawner = config.count / spawners.Count;
                int extraEnemies = config.count % spawners.Count; // Остаток для первого спавнера

                for (int i = 0; i < spawners.Count; i++)
                {
                    int count = enemiesPerSpawner + (i == 0 ? extraEnemies : 0);
                    if (count > 0)
                    {
                        spawners[i].QueueEnemies(config.enemyPrefab, count, currentWave.spawnInterval);
                    }
                }
            }

            // Ждем, пока все враги в волне будут заспавнены и уничтожены
            while (GameObject.FindObjectsOfType<EnemyAI>().Length > 0 || spawners.Exists(s => s.IsSpawning()))
            {
                yield return null;
            }

            isWaveInProgress = false;
            currentWaveIndex++;
            UpdateWaveText();

            // Если волны закончились
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