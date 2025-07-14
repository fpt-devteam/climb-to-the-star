public interface IPlayerMovement
{
    void Initialize(PlayerStats playerStats);
    void Move(float direction);
    void MovementInAir(float direction);
    void Jump();
    void Fall();
}
