using UnityEngine;

public interface IPlayerInput
{
  float GetMovementInput();

  // NEW: Vertical input for 8-directional dashing
  float GetVerticalInput();
  Vector2 GetDirectionalInput(); // Combined horizontal and vertical

  bool IsJumpPressed();
  bool IsJumpHeld();
  bool IsShieldPressed();
  bool IsShieldHeld();
  bool IsChargePressed();
  bool IsChargeHeld();
  bool IsAttackPressed();
  bool IsAttackHeld();
  bool IsDashPressed();
  bool IsPausePressed();
}
