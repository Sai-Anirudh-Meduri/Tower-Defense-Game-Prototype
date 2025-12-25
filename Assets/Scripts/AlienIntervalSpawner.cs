using System.Collections;
using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    [Header("Alien Prefabs")]
    [SerializeField] private GameObject fastMartianAlienPrefab;
    [SerializeField] private GameObject buffMartianAlienPrefab;

    [Header("Spawn Settings")]
    [SerializeField, Min(0)] private int fastAlienCount = 3;
    [SerializeField, Min(0)] private int buffAlienCount = 2;
    [SerializeField, Min(0f)] private float spawnRadius = 2f;
    [SerializeField, Min(0.1f)] private float spawnInterval = 1f;

    [Header("Spawn Control")]
    [Tooltip("If true, spawns start automatically when the game begins.")]
    [SerializeField] private bool autoStart = true;

    private bool spawning = false;

    private void Start()
    {
        if (autoStart)
            StartCoroutine(SpawnAllAliens());
    }

    public void StartSpawning()
    {
        if (!spawning)
            StartCoroutine(SpawnAllAliens());
    }

    private IEnumerator SpawnAllAliens()
    {
        spawning = true;

        // Spawn Fast Martians first
        for (int i = 0; i < fastAlienCount; i++)
        {
            SpawnAlien(fastMartianAlienPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        // Then Buff Martians
        for (int i = 0; i < buffAlienCount; i++)
        {
            SpawnAlien(buffMartianAlienPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    private void SpawnAlien(GameObject alienPrefab)
    {
        if (alienPrefab == null)
        {
            Debug.LogWarning($"Missing alien prefab reference on {name}!");
            return;
        }

        // Random offset within radius
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(
            transform.position.x + randomCircle.x,
            transform.position.y,
            transform.position.z + randomCircle.y
        );

        // Instantiate alien prefab
        GameObject alien = Instantiate(alienPrefab, spawnPos, Quaternion.identity);

        // Optional: set alien parent to this spawner for organization
        alien.transform.parent = transform;

        Debug.Log($"Spawned {alien.name} at {spawnPos}");
    }
}
