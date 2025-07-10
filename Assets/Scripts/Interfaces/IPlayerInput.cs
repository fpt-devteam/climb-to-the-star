public interface IPlayerInput
{
    float MovementInput { get; }
    bool IsJumpPressed { get; }
    bool IsJumpHeld { get; }
    bool IsShieldPressed { get; }
    bool IsShieldHeld { get; }
    bool IsSkill1Pressed { get; }
    bool IsSkill2Pressed { get; }
    bool IsSkill3Pressed { get; }
    bool IsSkill4Pressed { get; }
    bool IsPausePressed { get; }
}
