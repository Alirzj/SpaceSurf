using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;
    public GameObject[] collectablePrefabs;

    [Header("Shared Spawn Points")]
    public Transform[] spawnPoints; // 5 total, used by both obstacles and collectables

    [Header("Obstacle Spawn Timing (seconds)")]
    public float minObstacleInterval = 3f;
    public float maxObstacleInterval = 6f;

    [Header("Collectable Spawn Timing (seconds)")]
    public float minCollectableInterval = 8f;
    public float maxCollectableInterval = 15f;

    void Start()
    {
        // Start the two separate spawn loops
        StartCoroutine(ObstacleSpawnLoop());
        StartCoroutine(CollectableSpawnLoop());
    }

    IEnumerator ObstacleSpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(minObstacleInterval, maxObstacleInterval)); // Initial delay

        while (true)
        {
            SpawnObstacleSet(); // Spawn one obstacle set
            float waitTime = Random.Range(minObstacleInterval, maxObstacleInterval);
            yield return new WaitForSeconds(waitTime); // Wait before spawning next set
        }
    }

    IEnumerator CollectableSpawnLoop()
    {
        yield return new WaitForSeconds(Random.Range(minCollectableInterval, maxCollectableInterval)); // Initial delay

        while (true)
        {
            SpawnCollectable(); // Spawn one collectable
            float waitTime = Random.Range(minCollectableInterval, maxCollectableInterval);
            yield return new WaitForSeconds(waitTime); // Wait before next collectable
        }
    }

    void SpawnObstacleSet()
    {
        if (obstaclePrefabs.Length == 0 || spawnPoints.Length < 2)
            return;

        int safeIndex = Random.Range(0, spawnPoints.Length); // One lane is left empty for fairness

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (i == safeIndex)
                continue;

            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Instantiate(prefab, spawnPoints[i].position, Quaternion.identity);
        }
    }

    void SpawnCollectable()
    {
        if (collectablePrefabs.Length == 0 || spawnPoints.Length == 0)
            return;

        int index = Random.Range(0, spawnPoints.Length);
        GameObject prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
        Instantiate(prefab, spawnPoints[index].position, Quaternion.identity);
    }
}
