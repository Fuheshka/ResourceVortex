using UnityEngine;

public class GameManagerBootstrapper : MonoBehaviour
{
    [Header("Prefabs to Instantiate")]
    public GameObject deathScreenManagerPrefab;
    public GameObject enemySpawnerPrefab;
    public GameObject waveManagerPrefab;
    public GameObject playerControllerPrefab;

    private GameObject deathScreenManagerInstance;
    private GameObject enemySpawnerInstance;
    private GameObject waveManagerInstance;
    private GameObject playerControllerInstance;

    void Awake()
    {
        // Instantiate DeathScreenManager prefab if not present
        if (deathScreenManagerInstance == null && deathScreenManagerPrefab != null)
        {
            deathScreenManagerInstance = Instantiate(deathScreenManagerPrefab);
            deathScreenManagerInstance.name = deathScreenManagerPrefab.name;
        }

        // Instantiate EnemySpawner prefab if not present
        if (enemySpawnerInstance == null && enemySpawnerPrefab != null)
        {
            enemySpawnerInstance = Instantiate(enemySpawnerPrefab);
            enemySpawnerInstance.name = enemySpawnerPrefab.name;
        }

        // Instantiate WaveManager prefab if not present
        if (waveManagerInstance == null && waveManagerPrefab != null)
        {
            waveManagerInstance = Instantiate(waveManagerPrefab);
            waveManagerInstance.name = waveManagerPrefab.name;
        }

        // Instantiate PlayerController prefab if not present
        if (playerControllerInstance == null && playerControllerPrefab != null)
        {
            playerControllerInstance = Instantiate(playerControllerPrefab);
            playerControllerInstance.name = playerControllerPrefab.name;
        }
    }
}
