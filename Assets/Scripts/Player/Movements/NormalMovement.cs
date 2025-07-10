using UnityEngine;

public class NormalMovement : IPlayerMovement
{
    private Transform transform;
    private Rigidbody2D rb;

    private float jumpForce = 7f;
    private float moveSpeed = 7f;
    private float dashForce = 7f;

    public void Initialize(Player player)
    {
        this.rb = player.GetComponent<Rigidbody2D>();
        this.transform = player.transform;
    }

    public void Move(float direction)
    {
        transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);

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
        rb.AddForce(Vector2.right * direction * dashForce, ForceMode2D.Impulse);
    }
}
