public interface IPlayerMovement
{
    void Initialize(Player player);
    void Move(float direction);
    void Jump();
    void Dash(float direction);
    void Fall();
}
