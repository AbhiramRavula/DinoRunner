
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Ground Obstacles")]
    public GameObject smallCactusPrefab;
    public GameObject mediumCactusPrefab;
    public GameObject largeCactusPrefab;
    public GameObject stonePrefab;
    public GameObject dustBowlPrefab;

    [Header("Optional Flying Obstacle")]
    public GameObject birdPrefab;
    [Range(0f, 1f)] public float birdChance = 0.15f;
    [Tooltip("Y position for low birds: Dino must JUMP over")]
    public float birdLowY = -1.8f;
    [Tooltip("Y position for high birds: Dino must DUCK under")]
    public float birdHighY = 0.0f;

    [Header("Spawn Settings")]
    public float spawnX = 10f;
    public float groundY = -2.1f;
    public float minInterval = 1.8f;
    public float maxInterval = 2.8f;

    private float timer;
    private float nextSpawn;

    private void Start()
    {
        ScheduleNext();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        timer += Time.deltaTime;

        if (timer >= nextSpawn)
        {
            SpawnObstacle();
            timer = 0f;
            ScheduleNext();
        }
    }

    private void SpawnObstacle()
    {
        // Birds are optional. Leave birdPrefab empty to disable them.
        if (birdPrefab != null && Random.value < birdChance)
        {
            // Strictly select between a Low Bird (must jump over) and a High Bird (must duck under)
            float birdY = (Random.value < 0.5f) ? birdLowY : birdHighY;

            Instantiate(
                birdPrefab,
                new Vector3(spawnX, birdY, 0f),
                Quaternion.identity
            );

            return;
        }

        // Random ground obstacle.
        GameObject[] choices =
        {
            smallCactusPrefab,
            mediumCactusPrefab,
            largeCactusPrefab,
            stonePrefab,
            dustBowlPrefab
        };

        // Ignore prefab slots that haven't been assigned.
        int availableCount = 0;

        foreach (GameObject prefab in choices)
        {
            if (prefab != null)
                availableCount++;
        }

        if (availableCount == 0)
            return;

        int selectedIndex = Random.Range(0, availableCount);
        GameObject selectedPrefab = null;

        foreach (GameObject prefab in choices)
        {
            if (prefab == null)
                continue;

            if (selectedIndex == 0)
            {
                selectedPrefab = prefab;
                break;
            }

            selectedIndex--;
        }

        if (selectedPrefab != null)
        {
            Instantiate(
                selectedPrefab,
                new Vector3(spawnX, groundY, 0f),
                Quaternion.identity
            );
        }
    }

    private void ScheduleNext()
    {
        float speedMultiplier = (GameManager.Instance != null) ? GameManager.Instance.SpeedMultiplier : 1f;
        // Scale interval inversely so distance between obstacles stays consistent and fair at high speeds
        nextSpawn = Random.Range(minInterval, maxInterval) / Mathf.Max(speedMultiplier, 0.5f);
    }
}
