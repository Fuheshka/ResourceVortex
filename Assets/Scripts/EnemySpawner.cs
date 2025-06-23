using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public float spawnRadius = 10f; // Радиус спавна
    private Queue<SpawnTask> spawnQueue = new Queue<SpawnTask>(); // Очередь задач спавна
    private bool isSpawning = false;

    private class SpawnTask
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;

        public SpawnTask(GameObject prefab, int count, float interval)
        {
            this.enemyPrefab = prefab;
            this.count = count;
            this.spawnInterval = interval;
        }
    }

    public void QueueEnemies(GameObject enemyPrefab, int count, float spawnInterval)
    {
        spawnQueue.Enqueue(new SpawnTask(enemyPrefab, count, spawnInterval));
        if (!isSpawning)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        isSpawning = true;

        while (spawnQueue.Count > 0)
        {
            SpawnTask task = spawnQueue.Dequeue();
            for (int i = 0; i < task.count; i++)
            {
                SpawnEnemy(task.enemyPrefab);
                yield return new WaitForSeconds(task.spawnInterval);
            }
        }

        isSpawning = false;
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;

        // Raycast вниз для поиска поверхности
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition + Vector3.up * 50f, Vector3.down, out hit, 100f))
        {
            spawnPosition.y = hit.point.y;
        }
        else
        {
            spawnPosition.y = 0f; // Резервная высота
        }

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Проигрываем звук спавна
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.enemySpawnClip);
        }
    }

    public bool IsSpawning()
    {
        return isSpawning;
    }
}