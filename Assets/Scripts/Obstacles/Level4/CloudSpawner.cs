using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("Cloud Prefabs")]
    public GameObject[] cloudPrefabs;

    [Header("Spawn Settings")]
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 5f;
    public int minCloudsOnScreen = 2;
    public int maxCloudsOnScreen = 4;

    [Header("Spawn Area Settings")]
    public float spawnHeightOffset = -1f;
    public float spawnWidthExtend = 3f;

    [Header("Cloud Physics")]
    public Vector2 globalWindDirection = Vector2.right;
    public float windVariation = 1f;
    public float baseCloudSpeed = 1f;
    public float speedVariation = 0.5f;

    private float spawnTimer = 0f;
    private float currentSpawnInterval = 0f;
    private Camera mainCamera;
    private int currentCloudCount = 0;
    private Vector3 lastCameraPosition;
    private float cameraMovementThreshold = 5f;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        spawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
        currentSpawnInterval = spawnTimer;

        if (mainCamera != null)
        {
            lastCameraPosition = mainCamera.transform.position;
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        bool cameraHasMoved = false;
        if (mainCamera != null)
        {
            float cameraMovement = Vector3.Distance(mainCamera.transform.position, lastCameraPosition);
            if (cameraMovement > cameraMovementThreshold)
            {
                cameraHasMoved = true;
                lastCameraPosition = mainCamera.transform.position;
            }
        }

        int targetCloudCount = CalculateTargetCloudCount();

        if (currentCloudCount < minCloudsOnScreen || cameraHasMoved)
        {
            SpawnCloud();
            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        else if (spawnTimer >= currentSpawnInterval &&
            currentCloudCount < targetCloudCount)
        {
            SpawnCloud();
            spawnTimer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        UpdateCloudCount();
    }

    int CalculateTargetCloudCount()
    {

        float spawnRatio = Mathf.InverseLerp(maxSpawnInterval, minSpawnInterval, currentSpawnInterval);

        int targetCount = Mathf.RoundToInt(Mathf.Lerp(minCloudsOnScreen, maxCloudsOnScreen, spawnRatio));

        return Mathf.Clamp(targetCount, minCloudsOnScreen, maxCloudsOnScreen);
    }

    void SpawnCloud()
    {
        if (mainCamera == null || cloudPrefabs == null || cloudPrefabs.Length == 0)
            return;

        Vector3 spawnPosition = CalculateSpawnPosition();

        GameObject selectedPrefab = cloudPrefabs[Random.Range(0, cloudPrefabs.Length)];

        GameObject newCloud = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        SetupCloudPhysics(newCloud);

        newCloud.tag = "Cloud";

        currentCloudCount++;

        Debug.Log($"Cloud spawned! Current count: {currentCloudCount}, Target: {CalculateTargetCloudCount()}");
    }

    Vector3 CalculateSpawnPosition()
    {
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        Vector3 cameraPosition = mainCamera.transform.position;

        float spawnOffset = 2f; 
        bool spawnFromTop = Random.Range(0f, 1f) > 0.5f;

        if (spawnFromTop)
        {
            float minX = cameraPosition.x - cameraWidth / 2f - spawnOffset;
            float maxX = cameraPosition.x + cameraWidth / 2f + spawnOffset;
            float minY = cameraPosition.y + cameraHeight * 0.50f + spawnOffset * 0.2f; 
            float maxY = cameraPosition.y + cameraHeight * 0.50f + spawnOffset * 0.5f;  

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            return new Vector3(randomX, randomY, 0);
        }
        else
        {
       
            float minX = cameraPosition.x + cameraWidth * 0.50f + spawnOffset * 0.5f;  
            float maxX = cameraPosition.x + cameraWidth * 0.50f + spawnOffset;        
            float minY = cameraPosition.y - cameraHeight * 0.30f;  
            float maxY = cameraPosition.y + cameraHeight * 0.30f;  

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            return new Vector3(randomX, randomY, 0);
        }
    }

    void SetupCloudPhysics(GameObject cloud)
    {
        Cloud cloudScript = cloud.GetComponent<Cloud>();
        if (cloudScript != null)
        {
            Vector3 cloudPosition = cloud.transform.position;
            Vector3 cameraPosition = mainCamera.transform.position;

           
            Vector2 directionToCamera = (cameraPosition - cloudPosition).normalized;

         
            Vector2 windVariationVector = new Vector2(
                Random.Range(-windVariation * 0.3f, windVariation * 0.3f),
                Random.Range(-windVariation * 0.2f, windVariation * 0.2f)   
            );

            Vector2 finalWindDirection = (directionToCamera + windVariationVector).normalized;

            float cloudSpeed = baseCloudSpeed + Random.Range(-speedVariation, speedVariation);
            Vector2 initialVelocity = finalWindDirection * cloudSpeed;

            cloudScript.SetWindDirection(finalWindDirection);
            cloudScript.SetInitialVelocity(initialVelocity);
        }

        float randomScale = Random.Range(0.7f, 1.3f);
        cloud.transform.localScale = Vector3.one * randomScale;

        float randomRotation = Random.Range(-15f, 15f);
        cloud.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
    }

    void UpdateCloudCount()
    {
        GameObject[] clouds = GameObject.FindGameObjectsWithTag("Cloud");

        for (int i = 0; i < clouds.Length; i++)
        {
            if (clouds[i] != null)
            {
                Cloud cloudComponent = clouds[i].GetComponent<Cloud>();
                if (cloudComponent != null && cloudComponent.IsOutOfCameraBounds())
                {
                    Debug.Log($"Destroying cloud outside camera bounds at position {clouds[i].transform.position}");
                    cloudComponent.TriggerFadeOut();
                }
            }
        }

        clouds = GameObject.FindGameObjectsWithTag("Cloud");
        currentCloudCount = clouds.Length;
        Debug.Log($"Cloud count updated: {currentCloudCount} clouds active");
    }


    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;
        float spawnOffset = 2f; 

        // Vẽ camera bounds
        Gizmos.color = Color.green;
        Vector3 cameraCenter = new Vector3(cameraPosition.x, cameraPosition.y, 0);
        Gizmos.DrawWireCube(cameraCenter, new Vector3(cameraWidth, cameraHeight, 0));

        // Vẽ vùng spawn 1: Phần trên GẦN camera hơn
        Gizmos.color = Color.cyan;
        float topMinX = cameraPosition.x - cameraWidth / 2f - spawnOffset;
        float topMaxX = cameraPosition.x + cameraWidth / 2f + spawnOffset;
        float topMinY = cameraPosition.y + cameraHeight * 0.50f + spawnOffset * 0.2f;  
        float topMaxY = cameraPosition.y + cameraHeight * 0.50f + spawnOffset * 0.5f;  

        Vector3 topSpawnCenter = new Vector3((topMinX + topMaxX) / 2f, (topMinY + topMaxY) / 2f, 0);
        Vector3 topSpawnSize = new Vector3(topMaxX - topMinX, topMaxY - topMinY, 0);
        Gizmos.DrawWireCube(topSpawnCenter, topSpawnSize);

        Gizmos.color = Color.magenta;
        float rightMinX = cameraPosition.x + cameraWidth * 0.50f + spawnOffset * 0.5f;
        float rightMaxX = cameraPosition.x + cameraWidth * 0.50f + spawnOffset;
        float rightMinY = cameraPosition.y - cameraHeight * 0.30f;
        float rightMaxY = cameraPosition.y + cameraHeight * 0.30f;

        Vector3 rightSpawnCenter = new Vector3((rightMinX + rightMaxX) / 2f, (rightMinY + rightMaxY) / 2f, 0);
        Vector3 rightSpawnSize = new Vector3(rightMaxX - rightMinX, rightMaxY - rightMinY, 0);
        Gizmos.DrawWireCube(rightSpawnCenter, rightSpawnSize);

        Gizmos.color = Color.yellow;
        Vector3 windArrowStart = transform.position;
        Vector3 windArrowEnd = windArrowStart + (Vector3)globalWindDirection * 2f;
        Gizmos.DrawLine(windArrowStart, windArrowEnd);
        Gizmos.DrawWireSphere(windArrowEnd, 0.2f);

        Gizmos.color = Color.red;
        Vector3 topArrowStart = new Vector3(cameraPosition.x, topMinY, 0);
        Vector3 topArrowEnd = new Vector3(cameraPosition.x, cameraPosition.y + cameraHeight * 0.2f, 0);
        Gizmos.DrawLine(topArrowStart, topArrowEnd);

        Vector3 rightArrowStart = new Vector3(rightMinX, cameraPosition.y, 0);
        Vector3 rightArrowEnd = new Vector3(cameraPosition.x + cameraWidth * 0.2f, cameraPosition.y, 0);
        Gizmos.DrawLine(rightArrowStart, rightArrowEnd);
    }
}