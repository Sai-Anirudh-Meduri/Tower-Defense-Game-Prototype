using System.Collections;
using UnityEngine;

public class AlienWaveSpawner : MonoBehaviour
{
    [Header("Alien Prefabs")]
    public GameObject fastAlienPrefab;
    public GameObject buffAlienPrefab;

    [Header("Spawner Settings")]
    public Transform[] spawnPoints; 
    public float spawnRadius = 3f;
    public float spawnInterval = 1f;
    public float timeBetweenWaves = 5f;

    [Header("Wave Counts (by wave number)")]
    [Tooltip("Wave 1–6 Fast Martians")]
    public int[] waveFastCounts = { 3, 5, 7, 0, 4, 6 }; 

    [Tooltip("Wave 4–6 Buff Martians")]
    public int[] waveBuffCounts = { 0, 0, 0, 5, 2, 5 };

    [Header("Rock Wave Manager")]
    public RockWaveManager rockWaveManager;  // << 🔥 Added

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < waveFastCounts.Length)
        {
            yield return StartCoroutine(SpawnWave(currentWave));
            currentWave++;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("✅ All waves complete!");
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        isSpawning = true;
        Debug.Log($"🌊 Starting Wave {waveIndex + 1}");

        // 🔥 Spawn rocks for this wave immediately when the wave begins
        if (rockWaveManager != null)
        {
            rockWaveManager.SpawnRockWave(waveIndex);
        }
        else
        {
            Debug.LogWarning("⚠️ No RockWaveManager assigned.");
        }

        int fastCount = waveFastCounts[waveIndex];
        int buffCount = waveBuffCounts[waveIndex];

        // --- SPAWN FAST ALIENS ---
        for (int i = 0; i < fastCount; i++)
        {
            SpawnAlien(fastAlienPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        // --- SPAWN BUFF ALIENS ---
        for (int i = 0; i < buffCount; i++)
        {
            SpawnAlien(buffAlienPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log($"✅ Wave {waveIndex + 1} complete: {fastCount} Fast, {buffCount} Buff");

        isSpawning = false;
    }

    void SpawnAlien(GameObject alienPrefab)
    {
        if (spawnPoints.Length == 0 || alienPrefab == null)
        {
            Debug.LogWarning("⚠️ Missing spawner points or prefab reference!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );

        Vector3 spawnPosition = spawnPoint.position + randomOffset;

        GameObject newAlien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"👾 Spawned {newAlien.name} at {spawnPosition}");
    }
}
