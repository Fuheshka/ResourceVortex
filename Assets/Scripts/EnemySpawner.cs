using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float spawnRadius = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
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

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
