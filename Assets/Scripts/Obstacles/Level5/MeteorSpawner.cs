using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject meteorPrefab;
    public float minSpawnInterval = 40f;
    public float maxSpawnInterval = 60f;
    private float spawnTimer = 0f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= Random.Range(minSpawnInterval, maxSpawnInterval))
        {
            SpawnMeteor();
            spawnTimer = 0f;
        }
    }

    void SpawnMeteor()
    {
        if (mainCamera == null) return;

        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, mainCamera.nearClipPlane));
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float randomX = Random.Range(leftEdge.x, rightEdge.x);

        Vector3 spawnPosition = new Vector3(randomX, leftEdge.y, 0);

        Instantiate(meteorPrefab, spawnPosition, Quaternion.identity);
    }
}
