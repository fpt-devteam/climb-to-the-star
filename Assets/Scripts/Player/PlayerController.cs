using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField]
    private float jumpForce = 10f;

    [SerializeField]
    private float jumpCooldown = 0.5f;

    private Rigidbody2D _rigidbody;
    private Animator animator;

    private bool isGrounded = false;

    public StateMachine stateMachine;

    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        stateMachine = new StateMachine();

        var idleState = new IdleState(this, animator);
        var jumpState = new JumpState(this, animator);
        var fallState = new FallState(this, animator);
        var landingState = new LandingState(this, animator);
        var walkingState = new WalkingState(this, animator);

        stateMachine.AddTransition(idleState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(idleState, walkingState, new FuncPredicate(IsWalking));
        stateMachine.AddTransition(jumpState, fallState, new FuncPredicate(IsFalling));
        stateMachine.AddTransition(fallState, landingState, new FuncPredicate(IsGrounded));
        stateMachine.AddTransition(walkingState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(walkingState, idleState, new FuncPredicate(IsIdling));

        stateMachine.SetState(idleState);
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void HandleMovement()
    {
        var moveDirection = Input.GetAxisRaw("Horizontal");

        if (moveDirection > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDirection < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        _rigidbody.linearVelocity = new Vector2(
            moveDirection * moveSpeed,
            _rigidbody.linearVelocity.y
        );
    }

    public void HandleJump()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public bool IsWalking() => _rigidbody.linearVelocity.x != 0;

    public bool IsFalling() => _rigidbody.linearVelocity.y < -0.01f;

    public bool IsGrounded() => isGrounded;

    public bool CanJump() => isGrounded && Input.GetKeyDown(KeyCode.Space);

    public bool IsIdling() => _rigidbody.linearVelocity.x == 0 && isGrounded;
}
