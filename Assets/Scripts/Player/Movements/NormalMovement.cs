using System.Collections;
using UnityEngine;

public class NormalMovement : IPlayerMovement
{
    private Transform transform;
    private Rigidbody2D rb;

    private float jumpForce = 7f;
    private float moveSpeed = 7f;
    private float dashForce = 10f;

    public void Initialize(PlayerStats playerStats)
    {
        this.rb = playerStats.GetComponent<Rigidbody2D>();
        this.transform = playerStats.transform;
    }

    public void Move(float direction)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(transform.position.x + direction * moveSpeed, transform.position.y),
            moveSpeed * Time.fixedDeltaTime
        );
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void Fall()
    {
        //noop
    }

    public void Dash(float direction)
    {
        // StartCoroutine(HandleDashCoroutine(direction));
    }
}
