using UnityEngine;

public class Cloud : MonoBehaviour
{
    [Header("Cloud Physics Settings")]
    public float baseSpeed = 1f;
    public float windInfluence = 0.5f;
    public float gravityInfluence = 0.1f;
    public float turbulenceStrength = 0.3f;
    public float directionChangeInterval = 3f;

    [Header("Platform Attraction")]
    public LayerMask platformLayerMask = 1;
    public float attractionRadius = 15f;
    public float attractionStrength = 0.2f;
    public float platformDetectionInterval = 2f;

    [Header("Cloud Behavior")]
    public float lifeTime = 10f;       // Thời gian sống của cloud (0-10s)
    public float fadeOutTime = 1f;     // Chỉ dùng cho fade khi out of bounds

    private Vector2 velocity;
    private Vector2 windDirection = Vector2.right;
    private float timer = 0f;
    private float ageTimer = 0f;
    private float platformScanTimer = 0f;
    private Camera mainCamera;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer[] allSpriteRenderers;
    private float[] initialAlphas;
    private bool isFadingOut = false;
    private bool isDestroy = false;
    private Vector3 nearestPlatform = Vector3.zero;
    private bool hasPlatformTarget = false;

    [Header("Prefab Clouds disperse")]
    public GameObject cloudDispersePrefab;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        initialAlphas = new float[allSpriteRenderers.Length];

        if (allSpriteRenderers.Length > 0)
        {
            for (int i = 0; i < allSpriteRenderers.Length; i++)
            {
                initialAlphas[i] = allSpriteRenderers[i].color.a;
                Debug.Log($"Sprite {i} ({allSpriteRenderers[i].name}): Initial alpha = {initialAlphas[i]}");
            }

            Debug.Log($"Found {allSpriteRenderers.Length} SpriteRenderers in cloud prefab");
        }
        else
        {
            Debug.LogWarning("No SpriteRenderers found in cloud prefab!");
        }

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = gravityInfluence;
        rb.linearDamping = 0.5f; // Tạo sức cản không khí
        rb.angularDamping = 0.5f;

        velocity = new Vector2(
            Random.Range(-baseSpeed, baseSpeed),
            Random.Range(-baseSpeed * 0.5f, baseSpeed * 0.2f)
        );

        windDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f)).normalized;

        Debug.Log($"Cloud created with lifetime: {lifeTime}s, fade time: {fadeOutTime}s");
    }

    void FixedUpdate()
    {
        CalculatePhysicsMovement();

        rb.linearVelocity = velocity;

        CheckIfOutOfBounds();

        UpdateLifetime();
    }

    void Update()
    {
        timer += Time.deltaTime;
        ageTimer += Time.deltaTime;
        platformScanTimer += Time.deltaTime;

        if (timer >= directionChangeInterval)
        {
            windDirection = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-0.3f, 0.3f)
            ).normalized;

            timer = 0f;
        }

        if (platformScanTimer >= platformDetectionInterval)
        {
            ScanForNearestPlatform();
            platformScanTimer = 0f;
        }
    }

    void CalculatePhysicsMovement()
    {
        Vector2 windForce = windDirection * windInfluence;

        Vector2 turbulence = new Vector2(
            Mathf.PerlinNoise(Time.time * 0.5f, transform.position.y * 0.1f) - 0.5f,
            Mathf.PerlinNoise(transform.position.x * 0.1f, Time.time * 0.3f) - 0.5f
        ) * turbulenceStrength;

        Vector2 platformAttraction = Vector2.zero;
        if (hasPlatformTarget)
        {
            Vector2 directionToPlatform = ((Vector2)nearestPlatform - (Vector2)transform.position).normalized;
            float distanceToPlatform = Vector2.Distance(transform.position, nearestPlatform);

            float attractionFactor = Mathf.InverseLerp(attractionRadius, 0f, distanceToPlatform);
            platformAttraction = directionToPlatform * attractionStrength * attractionFactor;
        }

        Vector2 totalForce = windForce + turbulence + platformAttraction;
        velocity = Vector2.Lerp(velocity, totalForce, Time.fixedDeltaTime);

        velocity = Vector2.ClampMagnitude(velocity, baseSpeed * 2f);
    }

    void UpdateLifetime()
    {
        if (ageTimer >= lifeTime - fadeOutTime && !isFadingOut)
        {
            isFadingOut = true;
            Debug.Log($"Cloud starting fade out due to lifetime: {ageTimer}/{lifeTime}");
        }

        if (isFadingOut && allSpriteRenderers != null && allSpriteRenderers.Length > 0)
        {
            float fadeStartTime = lifeTime - fadeOutTime;
            float currentFadeTime = ageTimer - fadeStartTime;
            float fadeProgress = Mathf.Clamp01(currentFadeTime / fadeOutTime);

            for (int i = 0; i < allSpriteRenderers.Length; i++)
            {
                if (allSpriteRenderers[i] != null)
                {
                    float currentAlpha = Mathf.Lerp(initialAlphas[i], 0f, fadeProgress);

                    Color spriteColor = allSpriteRenderers[i].color;
                    spriteColor.a = currentAlpha;
                    allSpriteRenderers[i].color = spriteColor;
                }
            }

            if (Time.time % 0.2f < Time.deltaTime)
            {
                float mainAlpha = allSpriteRenderers.Length > 0 ? allSpriteRenderers[0].color.a : 0f;
                Debug.Log($"Fade progress: {fadeProgress:F2}, Main Alpha: {mainAlpha:F2} (from {(initialAlphas.Length > 0 ? initialAlphas[0] : 1f):F2})");
                Debug.Log($"Fading {allSpriteRenderers.Length} sprite renderers");
            }

            if (fadeProgress >= 1f && !isDestroy)
            {
                Debug.Log("Cloud destroyed after fade out");
                Destroy(gameObject);
            }
        }

        if (ageTimer >= lifeTime + fadeOutTime)
        {
            if (!isDestroy && cloudDispersePrefab != null)
            {
                Instantiate(cloudDispersePrefab, transform.position, Quaternion.identity);
            }
            Debug.Log("Cloud emergency destroy due to timeout");
            Destroy(gameObject);
        }
    }

    void ScanForNearestPlatform()
    {
        Collider2D[] platforms = Physics2D.OverlapCircleAll(transform.position, attractionRadius, platformLayerMask);

        if (platforms.Length > 0)
        {
            float nearestDistance = float.MaxValue;
            Vector3 nearestPosition = Vector3.zero;

            foreach (Collider2D platform in platforms)
            {
                float distance = Vector2.Distance(transform.position, platform.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPosition = platform.transform.position;
                }
            }

            nearestPlatform = nearestPosition;
            hasPlatformTarget = true;
        }
        else
        {
            hasPlatformTarget = false;
        }
    }

    void CheckIfOutOfBounds()
    {
        if (mainCamera == null) return;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Bounds checking responsive với camera movement
        if (viewportPos.x < -0.3f || viewportPos.x > 1.3f ||
            viewportPos.y < -0.3f || viewportPos.y > 1.3f)
        {
            if (!isFadingOut)
            {
                isFadingOut = true;
                Debug.Log($"Cloud starting fade out due to out of bounds: {viewportPos}, Camera pos: {mainCamera.transform.position}");
            }
        }
    }

    // Method để set velocity từ spawner
    public void SetInitialVelocity(Vector2 initialVelocity)
    {
        velocity = initialVelocity;
    }

    public void SetWindDirection(Vector2 wind)
    {
        windDirection = wind.normalized;
    }

    // Public methods for CloudSpawner
    public bool IsOutOfCameraBounds()
    {
        if (mainCamera == null) return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        return (viewportPos.x < -0.3f || viewportPos.x > 1.3f ||
                viewportPos.y < -0.3f || viewportPos.y > 1.3f);
    }

    public void TriggerFadeOut()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            Debug.Log($"Cloud fade out triggered by CloudSpawner at position {transform.position}");
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);

        if (hasPlatformTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, nearestPlatform);
            Gizmos.DrawWireSphere(nearestPlatform, 1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, velocity.normalized * 3f);
    }
}