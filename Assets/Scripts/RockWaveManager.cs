using System.Collections.Generic;
using UnityEngine;

public class RockWaveManager : MonoBehaviour
{
    [Header("Rock Prefabs (Rock1..Rock13)")]
    [Tooltip("Assign your rock prefabs here.")]
    public GameObject[] rockPrefabs;

    [Header("Rock Spawners (RockSpawner1..N)")]
    [Tooltip("Assign the transforms that act as rock spawn points.")]
    public Transform[] rockSpawnPoints;

    [Header("Rocks PER SPAWNER")]
    [Tooltip("Number of rocks to spawn at EACH spawner for the given wave. Total spawned = value * number of spawners.")]
    public int[] rocksPerSpawner;

    [Header("Spawn Settings")]
    [Tooltip("Randomize position inside this radius around the spawn point.")]
    public float spawnRadius = 1f;

    private List<GameObject> spawnedRocks = new List<GameObject>();

    /// <summary>
    /// Spawns the rocks for the given wave index (0-based).
    /// Clears any previously spawned rocks first.
    /// This will spawn rocksPerSpawner[waveIndex] rocks at EACH spawn point.
    /// </summary>
    public void SpawnRockWave(int waveIndex)
    {
        // Basic validation
        if (rockPrefabs == null || rockPrefabs.Length == 0)
        {
            Debug.LogWarning("[RockWaveManager] No rockPrefabs assigned.");
            return;
        }

        if (rockSpawnPoints == null || rockSpawnPoints.Length == 0)
        {
            Debug.LogWarning("[RockWaveManager] No rockSpawnPoints assigned.");
            return;
        }

        if (rocksPerSpawner == null || rocksPerSpawner.Length == 0)
        {
            Debug.LogWarning("[RockWaveManager] rocksPerSpawner not set.");
            return;
        }

        if (waveIndex < 0 || waveIndex >= rocksPerSpawner.Length)
        {
            Debug.LogWarning($"[RockWaveManager] waveIndex {waveIndex} is out of range (0..{rocksPerSpawner.Length - 1}). Clamping to valid range.");
            waveIndex = Mathf.Clamp(waveIndex, 0, rocksPerSpawner.Length - 1);
        }

        // Clear previous rocks
        ClearOldRocks();

        int perSpawner = Mathf.Max(0, rocksPerSpawner[waveIndex]);
        int totalToSpawn = perSpawner * rockSpawnPoints.Length;

        for (int s = 0; s < rockSpawnPoints.Length; s++)
        {
            Transform spawnPoint = rockSpawnPoints[s];

            for (int i = 0; i < perSpawner; i++)
            {
                // Choose random prefab
                GameObject prefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

                // random offset on XZ plane
                Vector3 offset = new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0f,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                Vector3 spawnPos = spawnPoint.position + offset;

                GameObject rock = Instantiate(prefab, spawnPos, Quaternion.identity);
                spawnedRocks.Add(rock);
            }
        }

        Debug.Log($"[RockWaveManager] Spawned {totalToSpawn} rocks ({perSpawner} per spawner) for wave {waveIndex + 1}.");
    }

    /// <summary>
    /// Destroys and clears all currently spawned rocks.
    /// </summary>
    public void ClearOldRocks()
    {
        for (int i = spawnedRocks.Count - 1; i >= 0; i--)
        {
            GameObject r = spawnedRocks[i];
            if (r != null) Destroy(r);
        }
        spawnedRocks.Clear();
    }

    /// <summary>
    /// Optional helper: return number of currently spawned rocks.
    /// </summary>
    public int CurrentSpawnedCount()
    {
        return spawnedRocks.Count;
    }
}
