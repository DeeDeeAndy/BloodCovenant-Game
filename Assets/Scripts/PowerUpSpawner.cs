using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public static PowerUpSpawner Instance { get; private set; }

    [Header("Power Up Prefabs")]
    public GameObject[] powerUpPrefabs;

    [Header("Spawn Settings")]
    public float spawnHeightOffset = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TrySpawnPowerUp(Vector3 position, float dropChance)
    {
        if (powerUpPrefabs.Length == 0) return;

        if (Random.value <= dropChance)
        {
            SpawnRandomPowerUp(position);
        }
    }

    void SpawnRandomPowerUp(Vector3 position)
    {
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        Vector3 spawnPos = position + Vector3.up * spawnHeightOffset;
        Instantiate(prefab, spawnPos, Quaternion.identity);

        Debug.Log("Power-up spawned!");
    }
}