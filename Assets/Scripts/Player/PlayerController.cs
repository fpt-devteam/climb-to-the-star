using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject AttackPoint => player.AttackPoint;

    public StateMachine stateMachine;

    private IPlayerInput playerInput;

    private IPlayerMovement playerMovement;

    private Player player;

    private Rigidbody2D rb;

    private Animator animator;

    private bool isGrounded = false;
    private bool isFacingRight = true;

    // factory (...conditions, map) => return Movement();
    // observer (...conditions, map) => return Movement();
    // map: state mua to vai lon, mua bth, bang, etc...

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
        playerInput = GetComponent<KeyboardInput>();

        playerMovement = new NormalMovement();
        playerMovement.Initialize(player);

        stateMachine = new StateMachine();

        var idleState = new IdleState(this);
        var jumpState = new JumpState(this);
        var fallState = new FallState(this);
        var landingState = new LandingState(this);
        var walkingState = new WalkingState(this);
        var attackState = new AttackState(this);
        var hurtState = new HurtState(this);
        var dieState = new DieState(this);
        var dashState = new DashState(this);
        var chargeIdleState = new ChargeIdleState(this);
        var shieldIdleState = new ShieldIdleState(this);
        var victoryState = new VictoryState(this);

        stateMachine.AddTransition(idleState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(idleState, walkingState, new FuncPredicate(IsWalking));
        stateMachine.AddTransition(idleState, attackState, new FuncPredicate(IsAttacking));
        stateMachine.AddTransition(idleState, dashState, new FuncPredicate(CanDash));

        stateMachine.AddTransition(idleState, hurtState, new FuncPredicate(player.IsHurt));

        stateMachine.AddTransition(
            idleState,
            chargeIdleState,
            new FuncPredicate(() => CanCharge() && IsCharging())
        );
        stateMachine.AddTransition(
            idleState,
            shieldIdleState,
            new FuncPredicate(() => CanShield() && IsShielding())
        );

        stateMachine.AddTransition(
            chargeIdleState,
            idleState,
            new FuncPredicate(() => !IsCharging())
        );

        stateMachine.AddTransition(
            shieldIdleState,
            idleState,
            new FuncPredicate(() => !IsShielding())
        );

        stateMachine.AddTransition(jumpState, fallState, new FuncPredicate(IsFalling));
        stateMachine.AddTransition(fallState, landingState, new FuncPredicate(IsGrounded));

        stateMachine.AddTransition(walkingState, idleState, new FuncPredicate(IsIdling));
        stateMachine.AddTransition(walkingState, jumpState, new FuncPredicate(CanJump));
        stateMachine.AddTransition(walkingState, attackState, new FuncPredicate(IsAttacking));
        stateMachine.AddTransition(walkingState, dashState, new FuncPredicate(CanDash));
        stateMachine.AddTransition(walkingState, chargeIdleState, new FuncPredicate(CanCharge));
        stateMachine.AddTransition(walkingState, shieldIdleState, new FuncPredicate(CanShield));

        stateMachine.SetState(idleState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        isFacingRight = playerInput.GetMovementInput() >= 0f;
    }

    public void HandleMovement() => playerMovement.Move(playerInput.GetMovementInput());

    public void HandleDash() => playerMovement.Dash(isFacingRight ? 1 : -1);

    public void HandleJump() => playerMovement.Jump();

    public void HandleCharge() => player.HandleCharge();

    public void StartShield() => player.StartShield();

    public void StopShield() => player.StopShield();

    public bool IsGrounded() => isGrounded;

    public bool IsWalking() => playerInput.GetMovementInput() != 0 && isGrounded;

    public bool IsIdling() => playerInput.GetMovementInput() == 0 && isGrounded;

    public bool IsShielding() => playerInput.IsShieldHeld();

    public bool IsCharging() => playerInput.IsChargeHeld();

    public bool IsFalling() => rb.linearVelocity.y < -0.01f;

    public bool IsAttacking() => IsGrounded() && playerInput.IsAttackPressed();

    public bool CanJump() => isGrounded && playerInput.IsJumpPressed();

    public bool CanDash() => isGrounded && playerInput.IsDashPressed();

    public bool CanCharge() => isGrounded && playerInput.IsChargePressed();

    public bool CanShield() => isGrounded && playerInput.IsShieldPressed();

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
}
