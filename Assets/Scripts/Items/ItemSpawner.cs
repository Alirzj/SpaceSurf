using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;
    public GameObject[] collectablePrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;                  // Obstacle spawn points
    public Transform[] collectableSpawnPoints;       // Assign separate points for collectables

    [Header("Timing Settings")]
    public float initialDelay = 10f;

    [Header("Obstacle Timing")]
    public float minObstacleDelay = 3f;
    public float maxObstacleDelay = 6f;

    [Header("Collectable Timing")]
    public float minCollectableDelay = 8f;
    public float maxCollectableDelay = 15f;

    void Start()
    {
        StartCoroutine(ObstacleSpawnLoop());
        StartCoroutine(CollectableSpawnLoop());
    }

    IEnumerator ObstacleSpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            int index = Random.Range(0, spawnPoints.Length);
            float delay = Random.Range(minObstacleDelay, maxObstacleDelay);

            yield return new WaitForSeconds(delay);
            SpawnObstacle(index);
        }
    }

    IEnumerator CollectableSpawnLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            int index = Random.Range(0, collectableSpawnPoints.Length);
            float delay = Random.Range(minCollectableDelay, maxCollectableDelay);

            yield return new WaitForSeconds(delay);
            SpawnCollectable(index);
        }
    }

    void SpawnObstacle(int spawnIndex)
    {
        if (obstaclePrefabs.Length == 0 || spawnPoints.Length == 0)
            return;

        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Instantiate(prefab, spawnPoints[spawnIndex].position, Quaternion.identity);
    }

    void SpawnCollectable(int spawnIndex)
    {
        if (collectablePrefabs.Length == 0 || collectableSpawnPoints.Length == 0)
            return;

        GameObject prefab = collectablePrefabs[Random.Range(0, collectablePrefabs.Length)];
        Instantiate(prefab, collectableSpawnPoints[spawnIndex].position, Quaternion.identity);
    }
}
