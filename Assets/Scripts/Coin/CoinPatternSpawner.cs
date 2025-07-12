using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinPatternSpawner : MonoBehaviour
{
    [Header("Coin Setup")]
    public GameObject coinPrefab;

    [Header("Spawn Points (Z & Y come from here)")]
    public Transform[] spawnPoints;

    [Header("X Spawn Range")]
    public float minX = -8f;
    public float maxX = 8f;

    [Header("Spawn Timing")]
    public float patternSpawnInterval = 6f;
    public float delayBetweenCoins = 0.1f;

    [Header("Pattern Shape Settings")]
    public float baseXSpacing = 2.2f;   // Controls spacing of X between coins
    public float arcRadius = 4f;        // Radius used in arc pattern

    void Start()
    {
        StartCoroutine(PatternSpawnLoop());
    }

    IEnumerator PatternSpawnLoop()
    {
        yield return new WaitForSeconds(3f); // Initial delay

        while (true)
        {
            yield return StartCoroutine(SpawnCoinPattern());
            yield return new WaitForSeconds(patternSpawnInterval);
        }
    }

    IEnumerator SpawnCoinPattern()
    {
        if (coinPrefab == null || spawnPoints.Length == 0)
            yield break;

        // Pick a random spawn point for Y and Z
        Transform basePoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        float baseY = basePoint.position.y;
        float baseZ = basePoint.position.z;

        // Get pattern and clamp base X
        List<float> xOffsets = GetRandomXPattern();
        float patternWidth = GetPatternWidth(xOffsets);
        float baseX = Random.Range(minX + patternWidth / 2f, maxX - patternWidth / 2f);

        foreach (float xOffset in xOffsets)
        {
            Vector3 spawnPos = new Vector3(baseX + xOffset, baseY, baseZ);
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(delayBetweenCoins);
        }
    }

    float GetPatternWidth(List<float> xOffsets)
    {
        if (xOffsets.Count == 0) return 0f;

        float min = float.MaxValue;
        float max = float.MinValue;

        foreach (float x in xOffsets)
        {
            if (x < min) min = x;
            if (x > max) max = x;
        }

        return max - min;
    }

    List<float> GetRandomXPattern()
    {
        int patternIndex = Random.Range(0, 6);

        switch (patternIndex)
        {
            case 0: return HorizontalLine(5);
            case 1: return Diagonal(4);
            case 2: return Diagonal(4, true);
            case 3: return VShape(5);
            case 4: return Arc(6);
            case 5: return ZigZag(5);
            default: return HorizontalLine(3);
        }
    }

    List<float> HorizontalLine(int count)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < count; i++)
        {
            result.Add((i - count / 2f) * baseXSpacing);
        }
        return result;
    }

    List<float> Diagonal(int count, bool reverse = false)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < count; i++)
        {
            float x = (reverse ? -1 : 1) * (i - count / 2f) * baseXSpacing;
            result.Add(x);
        }
        return result;
    }

    List<float> VShape(int count)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < count; i++)
        {
            float x = (i - count / 2f) * baseXSpacing;
            result.Add(x);
        }
        return result;
    }

    List<float> Arc(int count)
    {
        List<float> result = new List<float>();
        float angleStep = 180f / (count - 1);

        for (int i = 0; i < count; i++)
        {
            float angle = Mathf.Deg2Rad * (-90f + i * angleStep);
            float x = Mathf.Cos(angle) * arcRadius;
            result.Add(x);
        }

        return result;
    }

    List<float> ZigZag(int count)
    {
        List<float> result = new List<float>();
        for (int i = 0; i < count; i++)
        {
            float x = (i - count / 2f) * baseXSpacing;
            result.Add(x);
        }
        return result;
    }
}
