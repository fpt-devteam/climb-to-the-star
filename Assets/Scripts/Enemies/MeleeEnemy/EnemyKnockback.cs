using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Visual Effects")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.red;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyController enemyController;
    private EnemyStats enemyStats;
    private Color originalColor;

    // Knockback state
    private bool isBeingKnockedBack = false;
    private bool isStunned = false;
    private Vector2 knockbackVelocity;
    private float knockbackTimer;
    private float stunTimer;

    // State machine interface
    public bool IsKnockedBack => isBeingKnockedBack;
    public bool IsStunned => isStunned;
    public bool CanMove => !isBeingKnockedBack && !isStunned;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyController = GetComponent<EnemyController>();
        enemyStats = GetComponent<EnemyStats>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Update()
    {
        UpdateKnockback();
        UpdateStun();
    }

    private void UpdateKnockback()
    {
        if (!isBeingKnockedBack) return;

        knockbackTimer += Time.deltaTime;
        float progress = knockbackTimer / knockbackDuration;

        if (progress >= 1f)
        {
            // Knockback complete
            EndKnockback();
            return;
        }

        // Apply knockback velocity with curve
        float curveValue = knockbackCurve.Evaluate(progress);
        Vector2 currentVelocity = knockbackVelocity * curveValue;

        // Apply to rigidbody
        rb.linearVelocity = new Vector2(currentVelocity.x, rb.linearVelocity.y);
    }

    private void UpdateStun()
    {
        if (!isStunned) return;

        stunTimer += Time.deltaTime;

        if (stunTimer >= stunDuration)
        {
            EndStun();
        }
    }

    /// <summary>
    /// Apply knockback to the enemy from a source position
    /// </summary>
    /// <param name="sourcePosition">Position of the attack source</param>
    /// <param name="forceMultiplier">Multiplier for knockback force</param>
    public void ApplyKnockback(Vector3 sourcePosition, float forceMultiplier = 1f)
    {
        // Calculate knockback direction
        Vector2 knockbackDirection = (transform.position - sourcePosition).normalized;

        // Apply knockback
        ApplyKnockback(knockbackDirection, forceMultiplier);
    }

    /// <summary>
    /// Apply knockback in a specific direction
    /// </summary>
    /// <param name="direction">Knockback direction (will be normalized)</param>
    /// <param name="forceMultiplier">Multiplier for knockback force</param>
    public void ApplyKnockback(Vector2 direction, float forceMultiplier = 1f)
    {
        // Normalize direction
        direction = direction.normalized;

        // Calculate knockback velocity
        float finalForce = knockbackForce * forceMultiplier;
        knockbackVelocity = direction * finalForce;

        // Start knockback
        isBeingKnockedBack = true;
        knockbackTimer = 0f;

        // Start stun
        isStunned = true;
        stunTimer = 0f;

        // Visual feedback
        StartCoroutine(FlashEffect());

        Debug.Log($"Enemy {gameObject.name} knocked back with force {finalForce} in direction {direction}");
    }

    private IEnumerator FlashEffect()
    {
        if (spriteRenderer == null) yield break;

        // Flash red briefly
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        // Return to original color
        spriteRenderer.color = originalColor;
    }

    private void EndKnockback()
    {
        isBeingKnockedBack = false;
        knockbackTimer = 0f;

        // Stop horizontal movement
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void EndStun()
    {
        isStunned = false;
        stunTimer = 0f;

        Debug.Log($"Enemy {gameObject.name} stun ended");
    }

    /// <summary>
    /// Force end all knockback and stun effects
    /// </summary>
    public void ForceEnd()
    {
        EndKnockback();
        EndStun();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    /// <summary>
    /// Check if enemy can perform actions (move, attack, etc.)
    /// </summary>
    public bool CanPerformActions()
    {
        return !isBeingKnockedBack && !isStunned && !enemyStats.IsHurt;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize knockback range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, knockbackForce * 0.1f);

        if (isBeingKnockedBack)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, knockbackVelocity.normalized * 2f);
        }
    }
}
