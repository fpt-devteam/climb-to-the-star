using System.Collections;
using UnityEngine;

public class NormalMovement : IPlayerMovement
{
    private Transform transform;
    private Rigidbody2D rb;

    private float jumpForce = 7f;
    private float moveSpeed = 7f;
    private float moveSpeedInAir = 5f;
    private float airMoveSpeed = 3f;
    private float maxFallSpeed = 10f;

    public void Initialize(PlayerStats playerStats)
    {
        this.rb = playerStats.GetComponent<Rigidbody2D>();
        this.transform = playerStats.transform;

        jumpForce = playerStats.JumpForce;
        moveSpeed = playerStats.MoveSpeed;
    }

    public void Move(float direction)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(transform.position.x + direction * moveSpeed, transform.position.y),
            moveSpeed * Time.fixedDeltaTime
        );
    }

    public void MovementInAir(float direction)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(transform.position.x + direction * moveSpeedInAir, transform.position.y),
            moveSpeedInAir * Time.fixedDeltaTime
        );
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Fall()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
    }
}
