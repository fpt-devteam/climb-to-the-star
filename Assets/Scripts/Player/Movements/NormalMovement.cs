using System.Collections;
using UnityEngine;

public class NormalMovement : IPlayerMovement
{
    private Transform transform;
    private Rigidbody2D rb;
    private PlayerStats playerStats;

    public void Initialize(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
        this.rb = playerStats.GetComponent<Rigidbody2D>();
        this.transform = playerStats.transform;
    }

    public void Move(float direction)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(
                transform.position.x + direction * playerStats.MoveSpeed,
                transform.position.y
            ),
            playerStats.MoveSpeed * Time.fixedDeltaTime
        );
    }

    public void MovementInAir(float direction)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(
                transform.position.x + direction * playerStats.MoveSpeedInAir,
                transform.position.y
            ),
            playerStats.MoveSpeedInAir * Time.fixedDeltaTime
        );
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * playerStats.JumpForce, ForceMode2D.Impulse);
    }

    public void Fall()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, -playerStats.MaxFallSpeed)
        );
    }
}
