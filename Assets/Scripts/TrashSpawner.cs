using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public float spawnInterval = 5f;
    public float spawnRadius = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTrash();
            timer = 0f;
        }
    }

    void SpawnTrash()
    {
        if (trashPrefabs.Length == 0) return;

        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = transform.position.y; // Adjust to ground level for trash

        int index = Random.Range(0, trashPrefabs.Length);
        Instantiate(trashPrefabs[index], spawnPosition, Quaternion.identity);
    }
}
