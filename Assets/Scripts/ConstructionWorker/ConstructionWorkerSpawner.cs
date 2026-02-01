using UnityEngine;

public class ConstructionWorkerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject constructionWorkerPrefab;
    [SerializeField] private GameObject bus;
    
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float initialSpawnInterval = 5f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float spawnRateIncreasePerSecond = 0.1f;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject finishScreen;

    private float currentSpawnInterval;
    private float timeSinceLastSpawn = 0f;
    private float elapsedTime = 0f;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timeSinceLastSpawn += Time.deltaTime;

        // Increase spawn rate over time
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval,
            initialSpawnInterval - (elapsedTime * spawnRateIncreasePerSecond)
        );

        // Check if it's time to spawn
        if (timeSinceLastSpawn >= currentSpawnInterval)
        {
            SpawnConstructionWorker();
            timeSinceLastSpawn = 0f;
        }
    }

    private void SpawnConstructionWorker()
    {
        if (constructionWorkerPrefab == null || bus == null)
        {
            Debug.LogWarning("Construction worker prefab or bus reference is missing!");
            return;
        }

        // Get random position in radius around bus
        Vector3 spawnPosition = GetRandomSpawnPosition();

        // Instantiate the worker at the spawn position
        GameObject worker = Instantiate(constructionWorkerPrefab, spawnPosition, Quaternion.identity);
        
        // Set the bus target for the worker - use GetComponentInChildren since script is on a child
        ConstructionWorker workerScript = worker.GetComponentInChildren<ConstructionWorker>();
        if (workerScript != null)
        {
            workerScript.SetBusTarget(bus, healthBar, finishScreen);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Get random point in a circle around the bus
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 busPosition = bus.transform.position;

        // Keep the Y position same as bus, randomize X and Z
        return new Vector3(
            busPosition.x + randomCircle.x,
            busPosition.y,
            busPosition.z + randomCircle.y
        );
    }
}
