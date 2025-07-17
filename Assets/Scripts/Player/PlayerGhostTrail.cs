using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGhostTrail : MonoBehaviour
{
  [Header("Ghost Trail Settings")]
  [Range(0.02f, 0.2f)]
  [SerializeField] private float delaySeconds = 0.05f; // Faster spawn for smooth trail

  [Header("Professional Visual Effects")]
  [Range(0.2f, 0.7f)]
  [SerializeField] private float initialAlpha = 0.4f; // Start semi-transparent
  [Range(0.2f, 0.6f)]
  [SerializeField] private float ghostLifetime = 0.3f; // Short lifetime for snappy feel
  [SerializeField] private Color ghostTint = new Color(1f, 1f, 1f, 1f); // Pure white for clarity
  [SerializeField] private bool useEaseOutFade = true; // Smooth professional fade

  [Header("Performance Settings")]
  [Range(3, 8)]
  [SerializeField] private int maxGhostCount = 6; // Limit active ghosts for performance

  private float currentDelaySeconds;
  [SerializeField] private GameObject ghostPrefab;
  private bool canSpawnGhost;
  private SpriteRenderer playerSR;
  private PlayerController playerController;
  private int activeGhostCount = 0;

  private void Awake()
  {
    playerSR = GetComponentInChildren<SpriteRenderer>();
    playerController = GetComponent<PlayerController>();
  }

  private void Update()
  {
    if (canSpawnGhost && activeGhostCount < maxGhostCount)
    {
      spawnGhost();
    }
  }

  private void spawnGhost()
  {
    if (currentDelaySeconds >= delaySeconds)
    {
      // Create ghost at player position with proper rotation
      Vector3 ghostPosition = transform.position;
      Quaternion ghostRotation = transform.rotation;

      GameObject playerGhost = Instantiate(ghostPrefab, ghostPosition, ghostRotation);
      SpriteRenderer ghostSR = playerGhost.GetComponent<SpriteRenderer>();

      // Copy sprite and all visual properties from player
      ghostSR.sprite = playerSR.sprite;
      ghostSR.flipX = playerSR.flipX;

      // CRITICAL: Copy the player's scale to match facing direction
      playerGhost.transform.localScale = transform.localScale;

      // Apply immediate transparency with professional tinting
      Color ghostColor = ghostTint;
      ghostColor.a = initialAlpha; // Start semi-transparent immediately
      ghostSR.color = ghostColor;

      // Set proper sorting layer to appear behind player
      ghostSR.sortingOrder = playerSR.sortingOrder - 1;

      currentDelaySeconds = 0;
      activeGhostCount++;

      // Add optimized fade out effect
      GhostFade fadeOut = playerGhost.AddComponent<GhostFade>();
      fadeOut.Initialize(ghostLifetime, initialAlpha, useEaseOutFade, this);

      Destroy(playerGhost, ghostLifetime + 0.1f); // Slight buffer for cleanup
    }
    else
    {
      currentDelaySeconds += Time.deltaTime;
    }
  }

  private Quaternion GetGhostRotation()
  {
    // Use the player's current rotation for proper facing direction
    return transform.rotation;
  }

  public void setSpawnGhost(bool value)
  {
    canSpawnGhost = value;

    // Reset delay when starting for immediate first ghost
    if (value)
    {
      currentDelaySeconds = delaySeconds; // Immediate spawn
      activeGhostCount = 0; // Reset count
    }
  }

  // Called by ghost fade component when ghost is destroyed
  public void OnGhostDestroyed()
  {
    activeGhostCount = Mathf.Max(0, activeGhostCount - 1);
  }
}

public class GhostFade : MonoBehaviour
{
  private SpriteRenderer spriteRenderer;
  private float fadeOutTime;
  private float startAlpha;
  private float timer;
  private bool useEaseOut;
  private PlayerGhostTrail trailManager;

  public void Initialize(float lifetime, float initialAlpha, bool easeOut, PlayerGhostTrail manager)
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    fadeOutTime = lifetime;
    startAlpha = initialAlpha;
    useEaseOut = easeOut;
    trailManager = manager;
    timer = 0f;
  }

  private void Update()
  {
    if (spriteRenderer == null) return;

    timer += Time.deltaTime;
    float normalizedTime = timer / fadeOutTime;

    // Professional easing for smooth visual fade
    float easedTime = useEaseOut ? EaseOutQuad(normalizedTime) : normalizedTime;

    // Fade from initial alpha to 0 with smooth curve
    float currentAlpha = Mathf.Lerp(startAlpha, 0f, easedTime);

    Color color = spriteRenderer.color;
    color.a = currentAlpha;
    spriteRenderer.color = color;

    // Destroy when fully faded for performance
    if (normalizedTime >= 1f)
    {
      if (trailManager != null)
        trailManager.OnGhostDestroyed();

      Destroy(gameObject);
    }
  }

  // Professional easing function for smooth fade
  private float EaseOutQuad(float t)
  {
    return 1f - (1f - t) * (1f - t);
  }
}
