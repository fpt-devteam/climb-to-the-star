using UnityEngine;

public interface IPlayerMovement
{
  void Initialize(PlayerStats playerStats);
  void Move(float direction);
  void MovementInAir(float direction);
  void Jump();
  void Fall();

  // Enhanced platformer features
  void UpdateCoyoteTime(bool isGrounded);
  void UpdateJumpBuffer(bool jumpPressed);
  bool CanCoyoteJump();
  bool HasJumpBuffer();
  void CutJump();
  bool IsJumpCut();

  // External velocity control
  void SetExternalVelocityControl(float duration);
  void SetExternalVelocity(Vector2 velocity);
  void ClearExternalVelocityControl();
  bool IsUnderExternalControl();
  void ForceStopExternalControl(); // Emergency clear for stuck states

  // Landing mechanics
  void OnLanding();
  bool ShouldRollOnLanding();
  float GetLandingHorizontalSpeed();

  // Movement state queries
  float GetHorizontalSpeed();
  bool IsMovingHorizontally();
  Vector2 GetCurrentVelocity();
}
