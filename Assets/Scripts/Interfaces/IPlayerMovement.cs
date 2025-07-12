public interface IPlayerMovement
{
    void Initialize(PlayerStats playerStats);
    void Move(float direction);
    void Jump();
    void Dash(float direction);
    void Fall();
}
