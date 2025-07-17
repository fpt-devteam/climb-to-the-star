using UnityEngine;
using System.Collections;

public class ImpactEffects : MonoBehaviour
{
  [Header("Particle Prefabs")]
  [SerializeField] private GameObject hitSparksEffect;
  [SerializeField] private GameObject criticalHitEffect;
  [SerializeField] private GameObject blockEffect;

  [Header("Effect Settings")]
  [SerializeField] private float effectDuration = 1f;
  [SerializeField] private int maxEffectsPool = 20;

  // Static instance for easy access
  private static ImpactEffects instance;
  public static ImpactEffects Instance
  {
    get
    {
      if (instance == null)
        instance = FindFirstObjectByType<ImpactEffects>();
      return instance;
    }
  }

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(gameObject);
    }
  }

  /// <summary>
  /// Create hit sparks effect at position
  /// </summary>
  public static void HitSparks(Vector3 position, Vector3 direction)
  {
    if (Instance == null) return;

    Instance.CreateEffect(Instance.hitSparksEffect, position, direction);
  }

  /// <summary>
  /// Create critical hit effect at position
  /// </summary>
  public static void CriticalHit(Vector3 position, Vector3 direction)
  {
    if (Instance == null) return;

    Instance.CreateEffect(Instance.criticalHitEffect, position, direction);
  }

  /// <summary>
  /// Create block effect at position
  /// </summary>
  public static void Block(Vector3 position, Vector3 direction)
  {
    if (Instance == null) return;

    Instance.CreateEffect(Instance.blockEffect, position, direction);
  }

  private void CreateEffect(GameObject effectPrefab, Vector3 position, Vector3 direction)
  {
    if (effectPrefab == null) return;

    // Calculate rotation based on direction
    Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction);

    // Instantiate effect
    GameObject effect = Instantiate(effectPrefab, position, rotation);

    // Auto-destroy after duration
    StartCoroutine(DestroyAfterDelay(effect, effectDuration));

    Debug.Log($"Impact effect created at {position}");
  }

  private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
  {
    yield return new WaitForSeconds(delay);

    if (obj != null)
    {
      Destroy(obj);
    }
  }

  /// <summary>
  /// Create a simple colored flash effect as fallback
  /// </summary>
  public static void SimpleFlash(Vector3 position, Color color, float duration = 0.2f)
  {
    if (Instance == null) return;

    Instance.StartCoroutine(Instance.CreateFlashEffect(position, color, duration));
  }

  private IEnumerator CreateFlashEffect(Vector3 position, Color color, float duration)
  {
    // Create a simple sprite as flash effect
    GameObject flash = new GameObject("Flash Effect");
    flash.transform.position = position;

    SpriteRenderer sr = flash.AddComponent<SpriteRenderer>();
    sr.sprite = CreateFlashSprite();
    sr.color = color;

    // Fade out over time
    float timer = 0f;
    Color originalColor = color;

    while (timer < duration)
    {
      timer += Time.deltaTime;
      float alpha = Mathf.Lerp(originalColor.a, 0f, timer / duration);
      sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
      yield return null;
    }

    Destroy(flash);
  }

  private Sprite CreateFlashSprite()
  {
    // Create a simple white square sprite
    Texture2D texture = new Texture2D(32, 32);
    Color[] colors = new Color[32 * 32];

    for (int i = 0; i < colors.Length; i++)
    {
      colors[i] = Color.white;
    }

    texture.SetPixels(colors);
    texture.Apply();

    return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
  }
}
