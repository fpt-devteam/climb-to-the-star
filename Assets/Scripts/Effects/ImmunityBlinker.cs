using UnityEngine;

/// <summary>
/// Professional immunity blinking system that can be attached to any character.
/// Provides white blinking visual feedback during invincibility frames.
/// Used by both players and enemies for consistent visual language.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ImmunityBlinker : MonoBehaviour
{
  [Header("Professional Blinking Settings")]
  [SerializeField, Range(0.05f, 0.2f)]
  private float blinkInterval = 0.1f; // Fast professional blinking

  [SerializeField]
  private Color blinkColor = Color.white; // White flash color

  [SerializeField, Range(0.3f, 1.0f)]
  private float blinkIntensity = 0.8f; // How white the blink gets

  [Header("Debug")]
  [SerializeField]
  private bool showDebugLogs = false;

  // Component references
  private SpriteRenderer spriteRenderer;
  private Color originalColor;

  // Blinking state
  private float blinkTimer = 0f;
  private bool isBlinking = false;
  private bool isCurrentlyWhite = false;

  private void Awake()
  {
    // Get sprite renderer and store original color
    spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
      originalColor = spriteRenderer.color;
    }
    else
    {
      Debug.LogError($"ImmunityBlinker on {gameObject.name}: No SpriteRenderer found!");
    }
  }

  private void Update()
  {
    if (isBlinking && spriteRenderer != null)
    {
      UpdateBlinking();
    }
  }

  /// <summary>
  /// Start the immunity blinking effect
  /// </summary>
  public void StartBlinking()
  {
    if (spriteRenderer == null) return;

    isBlinking = true;
    blinkTimer = 0f;
    isCurrentlyWhite = false;

    // Start with original color
    spriteRenderer.color = originalColor;

    if (showDebugLogs)
    {
      Debug.Log($"ImmunityBlinker: Started blinking on {gameObject.name}");
    }
  }

  /// <summary>
  /// Stop the immunity blinking effect and restore original color
  /// </summary>
  public void StopBlinking()
  {
    if (spriteRenderer == null) return;

    isBlinking = false;
    blinkTimer = 0f;
    isCurrentlyWhite = false;

    // Ensure we return to original color
    spriteRenderer.color = originalColor;

    if (showDebugLogs)
    {
      Debug.Log($"ImmunityBlinker: Stopped blinking on {gameObject.name}");
    }
  }

  /// <summary>
  /// Check if currently blinking
  /// </summary>
  public bool IsBlinking => isBlinking;

  /// <summary>
  /// Update the original color (useful if the character's base color changes)
  /// </summary>
  public void RefreshOriginalColor()
  {
    if (spriteRenderer != null && !isBlinking)
    {
      originalColor = spriteRenderer.color;
    }
  }

  /// <summary>
  /// Set custom blink settings at runtime
  /// </summary>
  public void SetBlinkSettings(float interval, Color color, float intensity)
  {
    blinkInterval = Mathf.Clamp(interval, 0.05f, 0.2f);
    blinkColor = color;
    blinkIntensity = Mathf.Clamp01(intensity);
  }

  private void UpdateBlinking()
  {
    // Update blink timer
    blinkTimer += Time.deltaTime;

    // Toggle between original and blink color at intervals
    if (blinkTimer >= blinkInterval)
    {
      blinkTimer = 0f;

      if (isCurrentlyWhite)
      {
        // Switch back to original color
        spriteRenderer.color = originalColor;
        isCurrentlyWhite = false;
      }
      else
      {
        // Switch to white blink color
        Color blinkTargetColor = Color.Lerp(originalColor, blinkColor, blinkIntensity);
        spriteRenderer.color = blinkTargetColor;
        isCurrentlyWhite = true;
      }
    }
  }

  /// <summary>
  /// Force immediate color update (useful for debugging)
  /// </summary>
  public void ForceColorUpdate()
  {
    if (spriteRenderer != null)
    {
      if (isBlinking)
      {
        // If blinking, alternate color
        UpdateBlinking();
      }
      else
      {
        // If not blinking, ensure original color
        spriteRenderer.color = originalColor;
      }
    }
  }

  private void OnValidate()
  {
    // Clamp values in inspector
    blinkInterval = Mathf.Clamp(blinkInterval, 0.05f, 0.2f);
    blinkIntensity = Mathf.Clamp01(blinkIntensity);
  }

  private void OnDisable()
  {
    // Ensure we stop blinking when disabled
    if (isBlinking)
    {
      StopBlinking();
    }
  }
}
