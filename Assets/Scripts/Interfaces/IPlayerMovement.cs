public interface IPlayerMovement
{
    void Initialize(Player player);
    void Move(float moveDirection);
    void Jump();
    void Dash();
    void Fall();
}
