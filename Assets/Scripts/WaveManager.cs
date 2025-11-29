using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave Settings")]
    public List<WaveData> waves;
    public float timeBetweenWaves = 5f;
    public float spawnDelay = 0.5f;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float minSpawnDistance = 10f;

    [Header("References")]
    public Transform player;

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool waveInProgress = false;
    private bool gameOver = false;

    public event System.Action<int> OnWaveStart;
    public event System.Action<int> OnWaveComplete;
    public event System.Action OnAllWavesComplete;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartCoroutine(StartFirstWaveDelay());
    }

    IEnumerator StartFirstWaveDelay()
    {
        yield return new WaitForSeconds(2f);
        StartWave();
    }

    public void StartWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            OnAllWavesComplete?.Invoke();
            Debug.Log("All waves complete!");
            return;
        }

        waveInProgress = true;
        OnWaveStart?.Invoke(currentWaveIndex + 1);
        Debug.Log($"Wave {currentWaveIndex + 1} starting!");

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemySpawnInfo spawnInfo in wave.enemies)
        {
            for (int i = 0; i < spawnInfo.count; i++)
            {
                SpawnEnemy(spawnInfo.enemyPrefab);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPos = GetSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemiesAlive++;
        }
    }

    Vector3 GetSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return point.position;
        }

        if (player != null)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * minSpawnDistance;
            Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);
            return spawnPos;
        }

        return Vector3.zero;
    }

    public void OnEnemyDeath()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0 && waveInProgress)
        {
            waveInProgress = false;
            OnWaveComplete?.Invoke(currentWaveIndex + 1);
            Debug.Log($"Wave {currentWaveIndex + 1} complete!");

            currentWaveIndex++;

            if (currentWaveIndex < waves.Count)
            {
                StartCoroutine(WaveCompleteDelay());
            }
            else
            {
                OnAllWavesComplete?.Invoke();
                Debug.Log("All waves complete! Victory!");
            }
        }
    }

    IEnumerator WaveCompleteDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartWave();
    }

    public int GetCurrentWave() => currentWaveIndex + 1;
    public int GetTotalWaves() => waves.Count;
    public int GetEnemiesAlive() => enemiesAlive;
    public bool IsWaveInProgress() => waveInProgress;
}