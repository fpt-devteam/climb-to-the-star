using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    private PlayerStats playerStats;
    private IPlayerInput playerInput;
    private Rigidbody2D rb;

    private StateMachine stateMachine;
    private Dictionary<PlayerState, IState> states;
    private IPlayerMovement playerMovement;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isFacingRight = true;

    public PlayerStats PlayerStats => playerStats;
    public IPlayerInput PlayerInput => playerInput;
    public IPlayerMovement PlayerMovement => playerMovement;

    public bool IsFacingRight => isFacingRight;
    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateMachine();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();

        playerMovement = new NormalMovement();
        playerMovement.Initialize(playerStats);

        playerInput = new KeyboardInput();
    }

    private void InitializeStateMachine()
    {
        states = new Dictionary<PlayerState, IState>
        {
            { PlayerState.Locomotion, new LocomotionState(this) },
            { PlayerState.Charge, new ChargeState(this) },
            { PlayerState.Shield, new ShieldState(this) },
            { PlayerState.Dash, new DashState(this) },
            { PlayerState.Attack1, new Attack1State(this) },
            { PlayerState.Attack2, new Attack2State(this) },
            { PlayerState.Attack3, new Attack3State(this) },
            { PlayerState.Attack4, new Attack4State(this) },
            { PlayerState.Hurt, new HurtState(this) },
            { PlayerState.Land, new LandState(this) },
            { PlayerState.Die, new DieState(this) },
            { PlayerState.Jump, new JumpState(this) },
            { PlayerState.Fall, new FallState(this) },
        };

        stateMachine = new StateMachine();
        stateMachine.Initialize(GetState(PlayerState.Locomotion));
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        HandleFacingDirection();
        stateMachine.FixedUpdate();
    }

    private void HandleFacingDirection()
    {
        if (playerInput.GetMovementInput() > 0f && !isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
        else if (playerInput.GetMovementInput() < 0f && isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }
    }

    public bool IsWalking() => playerInput.GetMovementInput() != 0f && isGrounded;

    public bool IsIdling() =>
        !playerInput.IsChargeHeld()
        && !playerInput.IsShieldHeld()
        && playerInput.GetMovementInput() == 0f
        && isGrounded;

    public bool IsCharging() => playerInput.IsChargeHeld() && isGrounded;

    public bool IsShielding() => playerInput.IsShieldHeld() && isGrounded;

    public bool IsDashing() => playerInput.IsDashPressed() && isGrounded;

    public bool IsJumping()
    {
        if (IsFalling())
            return false;

        if (isJumping)
            return isJumping;

        if (isGrounded && playerInput.IsJumpPressed())
        {
            isJumping = true;
        }

        return isJumping;
    }

    public bool CanAttack() => isGrounded && playerInput.IsAttackPressed();

    public bool IsHurt() => playerStats.IsHurt;

    public bool IsFalling() => rb.linearVelocity.y < 0f && !isGrounded;

    public IState GetState(PlayerState state) =>
        states.TryGetValue(state, out IState stateInstance) ? stateInstance : null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
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
