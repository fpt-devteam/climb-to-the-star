public interface IPlayerInput
{
    float GetMovementInput();

    bool IsJumpPressed();

    bool IsJumpHeld();

    bool IsShieldPressed();

    bool IsShieldHeld();

    bool IsChargePressed();

    bool IsChargeHeld();

    bool IsAttackPressed();

    bool IsDashPressed();

    bool IsPausePressed();
}
