using UnityEngine;

public class MovementDebugger : MonoBehaviour
{
  [Header("Debug Settings")]
  [SerializeField] private bool showDebugInfo = true;
  [SerializeField] private bool showVelocityGizmo = true;
  [SerializeField] private bool showGroundCheck = true;

  private PlayerController playerController;
  private Rigidbody2D rb;
  private GUIStyle guiStyle;
  private GUIStyle headerStyle;

  private void Awake()
  {
    playerController = GetComponent<PlayerController>();
    rb = GetComponent<Rigidbody2D>();

    // Setup GUI styles
    guiStyle = new GUIStyle();
    guiStyle.fontSize = 11;
    guiStyle.normal.textColor = Color.white;

    headerStyle = new GUIStyle();
    headerStyle.fontSize = 12;
    headerStyle.fontStyle = FontStyle.Bold;
    headerStyle.normal.textColor = Color.yellow;
  }

  private void OnGUI()
  {
    if (!showDebugInfo) return;

    GUI.Box(new Rect(10, 10, 280, 240), "Dead Cells Style Movement Debug");

    GUILayout.BeginArea(new Rect(15, 30, 270, 210));

    // Get movement reference once for the entire method
    NormalMovement movement = playerController.PlayerMovement as NormalMovement;

    GUILayout.Label("=== VELOCITY ===", headerStyle);
    GUILayout.Label($"Velocity: {rb.linearVelocity:F2}", guiStyle);
    GUILayout.Label($"Speed: {rb.linearVelocity.magnitude:F2}", guiStyle);
    GUILayout.Label($"H-Speed: {Mathf.Abs(rb.linearVelocity.x):F2}", guiStyle);

    if (movement != null)
    {
      Vector2 currentVel = movement.GetCurrentVelocity();
      GUILayout.Label($"Direct Vel: {currentVel:F2}", guiStyle);
      GUILayout.Label($"Direct H-Speed: {Mathf.Abs(currentVel.x):F2}", guiStyle);
    }

    GUILayout.Space(5);
    GUILayout.Label("=== DASH ===", headerStyle);
    GUILayout.Label($"Can Dash: {playerController.IsDashing()}", guiStyle);
    GUILayout.Label($"Dash Cooldown: {playerController.DashCooldownRemaining:F2}s", guiStyle);
    GUILayout.Label($"On Cooldown: {playerController.IsDashOnCooldown}", guiStyle);

    GUILayout.Space(5);
    GUILayout.Label("=== PROFESSIONAL COMBAT ===", headerStyle);
    GUILayout.Label($"Can Attack: {playerController.CanAttack()}", guiStyle);
    GUILayout.Label($"Attack Pressed: {playerController.PlayerInput.IsAttackPressed()}", guiStyle);
    GUILayout.Label($"Attack Held: {playerController.PlayerInput.IsAttackHeld()}", guiStyle);

    // Show current state info
    var stateMachine = GetStateMachine();
    if (stateMachine != null && stateMachine.CurrentState != null)
    {
      string stateName = stateMachine.CurrentState.GetType().Name;
      GUILayout.Label($"Current State: {stateName}", guiStyle);

      if (stateName.Contains("Attack"))
      {
        GUILayout.Label($"Combat System: ACTIVE", guiStyle);

        // NEW: Show animation timing info for attack states
        if (stateMachine.CurrentState is BasePlayerAttackState attackState)
        {
          // Use reflection to access timing info (since fields are protected)
          var stateTimerField = typeof(BasePlayerAttackState).GetField("stateTimer",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          var hasMinTimeField = typeof(BasePlayerAttackState).GetField("hasMinAnimationTimePassed",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          var minDisplayTimeField = typeof(BasePlayerAttackState).GetField("minAnimationDisplayTime",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

          if (stateTimerField != null && hasMinTimeField != null && minDisplayTimeField != null)
          {
            float stateTimer = (float)stateTimerField.GetValue(attackState);
            bool hasMinTime = (bool)hasMinTimeField.GetValue(attackState);
            float minDisplayTime = (float)minDisplayTimeField.GetValue(attackState);

            GUILayout.Label($"State Timer: {stateTimer:F2}s", guiStyle);
            GUILayout.Label($"Min Display: {minDisplayTime:F2}s", guiStyle);
            GUILayout.Label($"Min Time Passed: {hasMinTime}", guiStyle);
          }
        }

        GUILayout.Label($"Press F2 for detailed combat debug", guiStyle);
      }
    }

    GUILayout.Label("=== STATES ===", headerStyle);
    GUILayout.Label($"Grounded: {playerController.IsGrounded}", guiStyle);
    GUILayout.Label($"Facing Right: {playerController.IsFacingRight}", guiStyle);
    GUILayout.Label($"Is Walking: {playerController.IsWalking()}", guiStyle);
    GUILayout.Label($"Is Idling: {playerController.IsIdling()}", guiStyle);

    GUILayout.Space(5);
    GUILayout.Label("=== INPUT DEBUG ===", headerStyle);
    GUILayout.Label($"Move Input: {playerController.PlayerInput.GetMovementInput():F2}", guiStyle);
    GUILayout.Label($"Dash Pressed: {playerController.PlayerInput.IsDashPressed()}", guiStyle);
    GUILayout.Label($"Can Dash: {playerController.IsDashing()}", guiStyle);

    if (movement != null)
    {
      GUILayout.Space(5);
      GUILayout.Label("=== VELOCITY CONTROL ===", headerStyle);
      GUILayout.Label($"External Control: {movement.IsUnderExternalControl()}", guiStyle);
      GUILayout.Label($"Moving Horizontally: {movement.IsMovingHorizontally()}", guiStyle);

      GUILayout.Space(5);
      GUILayout.Label("=== ENHANCED FEATURES ===", headerStyle);
      GUILayout.Label($"Can Coyote: {movement.CanCoyoteJump()}", guiStyle);
      GUILayout.Label($"Jump Buffer: {movement.HasJumpBuffer()}", guiStyle);
      GUILayout.Label($"Jump Cut: {movement.IsJumpCut()}", guiStyle);

      GUILayout.Space(5);
      GUILayout.Label("=== LANDING MECHANICS ===", headerStyle);
      GUILayout.Label($"Should Roll: {movement.ShouldRollOnLanding()}", guiStyle);
      GUILayout.Label($"Landing Speed: {movement.GetLandingHorizontalSpeed():F2}", guiStyle);
    }

    GUILayout.EndArea();
  }

  private void OnDrawGizmos()
  {
    if (!Application.isPlaying) return;

    // Draw velocity vector with better visualization
    if (showVelocityGizmo && rb != null)
    {
      // Horizontal velocity (green)
      Gizmos.color = Color.green;
      Vector3 hVelocityEnd = transform.position + Vector3.right * rb.linearVelocity.x * 0.1f;
      Gizmos.DrawLine(transform.position, hVelocityEnd);

      // Vertical velocity (red)
      Gizmos.color = Color.red;
      Vector3 vVelocityEnd = transform.position + Vector3.up * rb.linearVelocity.y * 0.1f;
      Gizmos.DrawLine(transform.position, vVelocityEnd);

      // Combined velocity (yellow)
      Gizmos.color = Color.yellow;
      Vector3 velocityEnd = transform.position + (Vector3)rb.linearVelocity * 0.1f;
      Gizmos.DrawLine(transform.position, velocityEnd);
      Gizmos.DrawWireSphere(velocityEnd, 0.05f);
    }

    // Draw ground check
    if (showGroundCheck && playerController != null)
    {
      Gizmos.color = playerController.IsGrounded ? Color.green : Color.red;
      Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.5f, 0.2f);
    }

    // Draw facing direction
    Gizmos.color = Color.blue;
    float direction = playerController != null && playerController.IsFacingRight ? 1f : -1f;
    Vector3 facingEnd = transform.position + Vector3.right * direction * 0.5f;
    Gizmos.DrawLine(transform.position, facingEnd);
    Gizmos.DrawWireCube(facingEnd, Vector3.one * 0.1f);
  }

  // Helper method to get state machine for debugging
  private StateMachine GetStateMachine()
  {
    // Use reflection to access private stateMachine field
    var field = typeof(PlayerController).GetField("stateMachine",
      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    return field?.GetValue(playerController) as StateMachine;
  }
}
