using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public float spawnInterval = 5f;
    public float spawnRadius = 10f;
    public float spawnHeight = 20f; // Fixed height above portal to spawn trash

    public int spawnCount = 1; // Number of trash prefabs to spawn per interval

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        // Clamp spawnInterval to minimum 0.5f seconds to avoid too rapid spawning
        float clampedSpawnInterval = Mathf.Max(spawnInterval, 0.5f);
        if (timer >= clampedSpawnInterval)
        {
            SpawnTrash();
            timer = 0f;
        }
    }

    void SpawnTrash()
    {
        if (trashPrefabs.Length == 0) return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(transform.position.x + randomCircle.x, transform.position.y + spawnHeight, transform.position.z + randomCircle.y);

            int index = Random.Range(0, trashPrefabs.Length);
            Instantiate(trashPrefabs[index], spawnPosition, Quaternion.identity);
        }
    }
}
