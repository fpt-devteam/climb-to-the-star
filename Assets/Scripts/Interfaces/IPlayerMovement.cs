using UnityEngine;

public interface IPlayerMovement
{
    void Initialize(Rigidbody2D rb, Transform transform);
    void HandleMovement(float moveDirection);
    void HandleJump();
    void HandleFall();
}
