using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

    [Header("프리팹 설정")]
    public GameObject obstaclePrefab;
    public GameObject trackingObstaclePrefab;
    public GameObject invincibleItemPrefab;
    public GameObject slowItemPrefab;

    [Header("생성 설정")]
    public float spawnInterval = 1.5f;
    public float minSpawnInterval = 0.4f;
    public float spawnIntervalDecrease = 0.1f;
    public float spawnY = 6f;
    public float spawnRangeX = 4f;
    public float trackingSpawnChance = 0.2f;

    [Header("아이템 생성 설정")]
    public float itemSpawnChance = 0.3f;        // 아이템 생성 확률
    public float invincibleItemChance = 0.5f;   // 아이템 중 무적 확률 (나머지는 슬로우)

    [Header("속도 설정")]
    public float initialSpeed = 3f;
    public float maxSpeed = 8f;
    public float speedIncreaseInterval = 10f;

    private float currentSpeed;
    private float currentSpawnInterval;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private bool isSpawning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentSpeed = initialSpeed;
        currentSpawnInterval = spawnInterval;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnCoroutine());
        StartCoroutine(SpeedUpCoroutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnCoroutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
            SpawnObject();
        }
    }

    IEnumerator SpeedUpCoroutine()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);
            currentSpawnInterval = Mathf.Max(
                currentSpawnInterval - spawnIntervalDecrease,
                minSpawnInterval
            );
            currentSpeed = Mathf.Min(currentSpeed + 0.5f, maxSpeed);
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0);

        // 아이템 생성 체크
        if (Random.value < itemSpawnChance)
        {
            // 무적 아이템 vs 슬로우 아이템
            if (Random.value < invincibleItemChance)
            {
                // 무적 아이템 생성
                if (invincibleItemPrefab != null)
                {
                    GameObject item = Instantiate(invincibleItemPrefab, spawnPos, Quaternion.identity);
                    activeObstacles.Add(item);
                    Debug.Log("무적 아이템 생성!");
                }
            }
            else
            {
                // 슬로우 아이템 생성
                if (slowItemPrefab != null)
                {
                    GameObject item = Instantiate(slowItemPrefab, spawnPos, Quaternion.identity);
                    activeObstacles.Add(item);
                    Debug.Log("슬로우 아이템 생성!");
                }
            }
            return;
        }

        // 장애물 생성
        bool isTracking = Random.value < trackingSpawnChance;
        GameObject prefab = isTracking ? trackingObstaclePrefab : obstaclePrefab;

        if (prefab != null)
        {
            GameObject obstacle = Instantiate(prefab, spawnPos, Quaternion.identity);
            Obstacle obs = obstacle.GetComponent<Obstacle>();
            if (obs != null)
            {
                obs.fallSpeed = currentSpeed;
                obs.isTracking = isTracking;
            }
            activeObstacles.Add(obstacle);
        }
    }

    public void SlowAllObstacles(float duration)
    {
        StartCoroutine(SlowCoroutine(duration));
    }

    IEnumerator SlowCoroutine(float duration)
    {
        foreach (GameObject obs in activeObstacles)
        {
            if (obs != null)
            {
                Obstacle obstacle = obs.GetComponent<Obstacle>();
                if (obstacle != null) obstacle.ApplySlow();
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (GameObject obs in activeObstacles)
        {
            if (obs != null)
            {
                Obstacle obstacle = obs.GetComponent<Obstacle>();
                if (obstacle != null) obstacle.RemoveSlow();
            }
        }
    }

    public void ClearAllObstacles()
    {
        foreach (GameObject obs in activeObstacles)
        {
            if (obs != null) Destroy(obs);
        }
        activeObstacles.Clear();
    }
}