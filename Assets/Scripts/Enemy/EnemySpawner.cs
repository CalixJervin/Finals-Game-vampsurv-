using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota; // Total enemies to spawn in this wave
        public float spawnInterval;
        public int spawnCount; // Enemies spawned so far
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public GameObject enemyPrefab;
        public int enemyCount; // Number of enemies of this type
        public int spawnCount; // Number of this type spawned so far
    }

    public List<Wave> waves;
    public int currentWaveCount; // Index of the current wave

    [Header("Spawner Attributes")]
    float spawnTimer;

    public float waveInterval; // Time between waves
    
    // IMPORTANT: Initialize to TRUE so the first wave starts immediately
    bool isWaveActive = true; 

    Transform player;

    float minSpawnDistance = 20f; // Minimum distance (Must be larger than screen!)
    public float maxSpawnDistance = 30f;

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    void Update()
    {
        // 1. SAFETY CHECK: If waves are empty or index is invalid, stop.
        if (currentWaveCount >= waves.Count) return;

        // 2. CHECK IF WAVE IS FINISHED
        // Condition: Spawned all enemies (spawnCount >= waveQuota) AND we are currently active
        if (waves[currentWaveCount].spawnCount >= waves[currentWaveCount].waveQuota && isWaveActive)
        {
            // Stop spawning and start the cooldown for the next wave
            StartCoroutine(BeginNextWave());
        }

        // 3. SPAWN LOGIC
        spawnTimer += Time.deltaTime;

        // Only spawn if active and timer is ready
        if (isWaveActive && spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            SpawnEnemies();
            spawnTimer = 0f;
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = false; // Stop spawning immediately

        // Wait for the wave interval (cooldown between waves)
        yield return new WaitForSeconds(waveInterval);

        // Increment Wave Index
        currentWaveCount++;

        // --- ENDLESS LOOP LOGIC ---
        // If we finished the last wave, loop back to the first one (Index 0)
        if (currentWaveCount >= waves.Count)
        {
            currentWaveCount = 0;
            Debug.Log("All waves complete! Looping back to Wave 1.");
        }

        // --- RESET COUNTERS ---
        // We must reset these to 0 so the wave can run again
        waves[currentWaveCount].spawnCount = 0;
        foreach (var group in waves[currentWaveCount].enemyGroups)
        {
            group.spawnCount = 0;
        }

        CalculateWaveQuota(); // Recalculate total for the new wave
        isWaveActive = true; // Resume spawning
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.Log("Wave " + currentWaveCount + " Quota: " + currentWaveQuota);
    }

    void SpawnEnemies()
    {
        // Safety check: Don't spawn if we hit the quota
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Find the first enemy type that hasn't finished spawning
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Random Position near player
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;

                    // 2. Pick a random distance (Between the "Donut hole" and the outer edge)
                    float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);

                    // 3. Calculate the final position
                    Vector3 spawnPosition = player.transform.position + (Vector3)(randomDirection * randomDistance);
                    
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    
                    return; // Spawn only 1 enemy per tick
                }
            }
        }
    }
}