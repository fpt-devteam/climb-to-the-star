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
  bool HasBufferedJump();
  void CutJump();
  bool IsJumpCut();

  // External velocity control
  void SetExternalVelocityControl(float duration);
  void SetExternalVelocity(Vector2 velocity);
  bool IsUnderExternalControl();
  void ForceStopExternalControl(); // Emergency clear for stuck states

  // Movement state queries
  bool IsMovingHorizontally();
  Vector2 GetVelocity();
  bool IsGrounded();
  bool IsAtJumpApex();
}
