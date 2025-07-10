using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerMovement playerMovement;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded = false;

    // factory (...conditions, map) => return Movement();
    // observer (...conditions, map) => return Movement();

    // map: state mua to vai lon, mua bth, bang, etc...

    [SerializeField]
    private Transform attackPoint;
    public Transform AttackPoint => attackPoint;

    public StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerMovement = new NormalMovement();
        playerMovement.Initialize(rb, transform);

        stateMachine = new StateMachine();

        var idleState = new IdleState(this, animator);
        var jumpState = new JumpState(this, animator);
        var fallState = new FallState(this, animator);
        var landingState = new LandingState(this, animator);
        var walkingState = new WalkingState(this, animator);
        var attackState = new AttackState(this, animator);

        stateMachine.AddTransition(idleState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(idleState, walkingState, new FuncPredicate(IsWalking));
        stateMachine.AddTransition(jumpState, fallState, new FuncPredicate(IsFalling));
        stateMachine.AddTransition(fallState, landingState, new FuncPredicate(IsGrounded));
        stateMachine.AddTransition(walkingState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(walkingState, idleState, new FuncPredicate(IsIdling));
        stateMachine.AddTransition(idleState, attackState, new FuncPredicate(IsAttacking));
        stateMachine.AddTransition(walkingState, attackState, new FuncPredicate(IsAttacking));

        stateMachine.SetState(idleState);
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void HandleMovement()
    {
        var moveDirection = Input.GetAxisRaw("Horizontal");
        playerMovement.HandleMovement(moveDirection);
    }

    public void HandleJump()
    {
        playerMovement.HandleJump();
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

    public bool IsWalking() => rb.linearVelocity.x != 0;

    public bool IsFalling() => rb.linearVelocity.y < -0.01f;

    public bool IsGrounded() => isGrounded;

    public bool CanJump() => isGrounded && Input.GetKeyDown(KeyCode.Space);

    public bool IsIdling() => rb.linearVelocity.x == 0 && isGrounded;

    public bool IsAttacking() => IsGrounded() && Input.GetKeyDown(KeyCode.Z);
}
