using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float maxShakeIntensity = 2f;
    [SerializeField] private float shakeDamping = 0.9f;
    [SerializeField] private AnimationCurve shakeFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Camera cam;
    private Vector3 originalPosition;
    private float currentShakeIntensity = 0f;
    private float shakeTimer = 0f;
    private float shakeDuration = 0f;

    // Static instance for easy access
    private static CameraShake instance;
    public static CameraShake Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<CameraShake>();
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cam = GetComponent<Camera>();
            originalPosition = transform.localPosition;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentShakeIntensity > 0f)
        {
            UpdateShake();
        }
    }

    private void UpdateShake()
    {
        shakeTimer += Time.deltaTime;

        // Calculate shake progress
        float progress = shakeTimer / shakeDuration;

        if (progress >= 1f)
        {
            // Shake complete
            StopShake();
            return;
        }

        // Apply falloff curve
        float falloffMultiplier = shakeFalloff.Evaluate(progress);
        float shakeAmount = currentShakeIntensity * falloffMultiplier;

        // Generate random shake offset
        Vector3 shakeOffset = new Vector3(
            Random.Range(-shakeAmount, shakeAmount),
            Random.Range(-shakeAmount, shakeAmount),
            0f
        );

        // Apply shake to camera
        transform.localPosition = originalPosition + shakeOffset;
    }

    /// <summary>
    /// Trigger camera shake with specified intensity and duration
    /// </summary>
    /// <param name="intensity">Shake intensity (0-1)</param>
    /// <param name="duration">Shake duration in seconds</param>
    public static void Shake(float intensity, float duration)
    {
        if (Instance != null)
        {
            Instance.TriggerShake(intensity, duration);
        }
    }

    /// <summary>
    /// Quick shake presets for common scenarios
    /// </summary>
    public static void LightHit() => Shake(0.3f, 0.15f);
    public static void MediumHit() => Shake(0.5f, 0.25f);
    public static void HeavyHit() => Shake(0.8f, 0.4f);
    public static void PlayerHurt() => Shake(0.6f, 0.3f);
    public static void EnemyDeath() => Shake(0.4f, 0.2f);
    public static void FinisherAttack() => Shake(1.0f, 0.5f);

    private void TriggerShake(float intensity, float duration)
    {
        currentShakeIntensity = Mathf.Clamp01(intensity) * maxShakeIntensity;
        shakeDuration = duration;
        shakeTimer = 0f;

        Debug.Log($"Camera shake triggered: intensity={intensity:F2}, duration={duration:F2}s");
    }

    private void StopShake()
    {
        currentShakeIntensity = 0f;
        shakeTimer = 0f;
        transform.localPosition = originalPosition;
    }

    /// <summary>
    /// Force stop any ongoing shake
    /// </summary>
    public static void Stop()
    {
        if (Instance != null)
        {
            Instance.StopShake();
        }
    }

    /// <summary>
    /// Update original position if camera parent moves
    /// </summary>
    public void UpdateOriginalPosition()
    {
        originalPosition = transform.localPosition;
    }
}
