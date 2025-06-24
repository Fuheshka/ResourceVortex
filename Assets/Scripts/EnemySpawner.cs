using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float spawnRadius = 10f;

    private float timer;

    private Queue<GameObject> enemyQueue = new Queue<GameObject>();
    private float currentSpawnInterval;
    private bool isSpawning = false;

    void Awake()
    {
        // Initialize enemy spawning when script awakes
        if (enemyPrefab != null)
        {
            QueueEnemies(enemyPrefab, 10, spawnInterval);
        }
    }

    void Update()
    {
        if (isSpawning && enemyQueue.Count > 0)
        {
            timer += Time.deltaTime;
            if (timer >= currentSpawnInterval)
            {
                SpawnEnemy(enemyQueue.Dequeue());
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
            isSpawning = false;
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Raycast down to find ground height
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            spawnPosition.y = hit.point.y;
        }
        else
        {
            spawnPosition.y = 0f; // fallback ground level
        }

        Instantiate(enemy, spawnPosition, Quaternion.identity);

        // Play enemy spawn sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemySpawnClip);
        }
    }

    public void QueueEnemies(GameObject prefab, int count, float interval)
    {
        for (int i = 0; i < count; i++)
        {
            enemyQueue.Enqueue(prefab);
        }
        currentSpawnInterval = interval;
        isSpawning = true;
    }

    public bool IsSpawning()
    {
        return isSpawning || enemyQueue.Count > 0;
    }
}
