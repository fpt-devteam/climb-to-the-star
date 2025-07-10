using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IPlayerInput playerInput;
    private IPlayerMovement playerMovement;
    private IPlayerSkill playerSkill;
    private Player player;

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
        player = GetComponent<Player>();
        playerInput = GetComponent<KeyboardInput>();

        playerMovement = new NormalMovement();
        playerMovement.Initialize(player);

        playerSkill = new Skill1();
        playerSkill.Initialize(player);

        stateMachine = new StateMachine();

        var idleState = new IdleState(this);
        var jumpState = new JumpState(this);
        var fallState = new FallState(this);
        var landingState = new LandingState(this);
        var walkingState = new WalkingState(this);
        var attackState = new AttackState(this);

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

    private void Update()
    {
        playerSkill.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void HandleMovement()
    {
        var moveDirection = Input.GetAxisRaw("Horizontal");
        playerMovement.Move(moveDirection);
    }

    public void HandleJump() => playerMovement.Jump();

    public void HandleDash() => playerMovement.Dash();

    public void HandleSkill() => playerSkill.Execute();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            player.TakeDamage(10f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public bool IsWalking() => playerInput.MovementInput != 0;

    public bool IsFalling() => rb.linearVelocity.y < -0.01f;

    public bool IsGrounded() => isGrounded;

    public bool CanJump() => isGrounded && playerInput.IsJumpPressed;

    public bool IsIdling() => playerInput.MovementInput == 0 && isGrounded;

    public bool IsAttacking() =>
        IsGrounded()
        && (
            playerInput.IsSkill1Pressed
            || playerInput.IsSkill2Pressed
            || playerInput.IsSkill3Pressed
            || playerInput.IsSkill4Pressed
        );
}
