using UnityEngine;

public class NormalMovement : IPlayerMovement
{
    private Rigidbody2D rb;
    private Transform transform;
    private float moveSpeed = 7f;
    private float jumpForce = 15f;

    public void Initialize(Rigidbody2D rb, Transform transform)
    {
        this.rb = rb;
        this.transform = transform;
    }

    public void HandleMovement(float moveDirection)
    {
        transform.localScale = new Vector3(Mathf.Sign(moveDirection), 1, 1);

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(transform.position.x + moveDirection * moveSpeed, transform.position.y),
            moveSpeed * Time.fixedDeltaTime
        );
    }

    public void HandleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void HandleFall() { }
}
