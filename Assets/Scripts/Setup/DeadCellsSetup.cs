using UnityEngine;

/// <summary>
/// Dead Cells Setup Helper - Automatically configures game objects for Dead Cells-style gameplay
/// Run this in the editor or at runtime to ensure all components are properly set up
/// </summary>
public class DeadCellsSetup : MonoBehaviour
{
  [Header("Setup Options")]
  [SerializeField] private bool setupOnAwake = true;
  [SerializeField] private bool verboseLogging = true;

  void Awake()
  {
    if (setupOnAwake)
    {
      SetupDeadCellsFeatures();
    }
  }

  [ContextMenu("Setup Dead Cells Features")]
  public void SetupDeadCellsFeatures()
  {
    Log("=== DEAD CELLS SETUP STARTING ===");

    SetupCameraShake();
    SetupPlayer();
    SetupEnemies();
    SetupImpactEffects();

    Log("=== DEAD CELLS SETUP COMPLETE ===");
  }

  private void SetupCameraShake()
  {
    Log("Setting up Camera Shake...");

    Camera mainCamera = Camera.main;
    if (mainCamera == null)
    {
      mainCamera = FindFirstObjectByType<Camera>();
    }

    if (mainCamera != null)
    {
      CameraShake cameraShake = mainCamera.GetComponent<CameraShake>();
      if (cameraShake == null)
      {
        cameraShake = mainCamera.gameObject.AddComponent<CameraShake>();
        Log("Added CameraShake component to main camera");
      }
    }
    else
    {
      LogWarning("No camera found for shake setup!");
    }
  }

  private void SetupPlayer()
  {
    Log("Setting up Player...");

    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player == null)
    {
      LogWarning("No Player found with 'Player' tag!");
      return;
    }

    // Ensure Rigidbody2D is properly configured
    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
      rb.gravityScale = 4f; // Dead Cells-style snappy gravity
      rb.freezeRotation = true;
      rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
      Log("Configured player Rigidbody2D for Dead Cells physics");
    }

    // Ensure ghost trail is present
    PlayerGhostTrail ghostTrail = player.GetComponent<PlayerGhostTrail>();
    if (ghostTrail == null)
    {
      ghostTrail = player.gameObject.AddComponent<PlayerGhostTrail>();
      Log("Added PlayerGhostTrail component");
    }

    Log("Player setup complete!");
  }

  private void SetupEnemies()
  {
    Log("Setting up Enemies...");

    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    foreach (GameObject enemy in enemies)
    {
      SetupSingleEnemy(enemy);
    }

    Log($"Setup complete for {enemies.Length} enemies");
  }

  private void SetupSingleEnemy(GameObject enemy)
  {
    // Ensure EnemyKnockback component
    EnemyKnockback knockback = enemy.GetComponent<EnemyKnockback>();
    if (knockback == null)
    {
      knockback = enemy.AddComponent<EnemyKnockback>();
      Log($"Added EnemyKnockback to {enemy.name}");
    }

    // Ensure proper Rigidbody2D setup
    Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
    if (rb == null)
    {
      rb = enemy.AddComponent<Rigidbody2D>();
      Log($"Added Rigidbody2D to {enemy.name}");
    }

    // Configure for proper physics
    rb.gravityScale = 1f;
    rb.freezeRotation = true;
    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
  }

  private void SetupImpactEffects()
  {
    Log("Setting up Impact Effects...");

    ImpactEffects effects = FindFirstObjectByType<ImpactEffects>();
    if (effects == null)
    {
      GameObject effectsGO = new GameObject("Impact Effects");
      effects = effectsGO.AddComponent<ImpactEffects>();
      Log("Created Impact Effects system");
    }
  }

  private void Log(string message)
  {
    if (verboseLogging)
    {
      Debug.Log($"[Dead Cells Setup] {message}");
    }
  }

  private void LogWarning(string message)
  {
    Debug.LogWarning($"[Dead Cells Setup] {message}");
  }

  [ContextMenu("Validate Dead Cells Setup")]
  public void ValidateSetup()
  {
    Log("=== VALIDATING DEAD CELLS SETUP ===");

    bool allGood = true;

    // Check camera shake
    if (FindFirstObjectByType<CameraShake>() == null)
    {
      LogWarning("CameraShake system not found!");
      allGood = false;
    }

    // Check impact effects
    if (FindFirstObjectByType<ImpactEffects>() == null)
    {
      LogWarning("ImpactEffects system not found!");
      allGood = false;
    }

    if (allGood)
    {
      Log("✓ All Dead Cells features are properly set up!");
    }
    else
    {
      LogWarning("✗ Some Dead Cells features need attention. Run Setup to fix.");
    }
  }
}
