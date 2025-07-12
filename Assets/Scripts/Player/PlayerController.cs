using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Walk,
    Jump,
    Fall,
    Land,
    Attack,
    Hurt,
    Die,
    Dash,
    Charge,
    Shield,
    Victory,
}

public class PlayerController : MonoBehaviour
{
    public PlayerStats PlayerStats { get; private set; }
    public IPlayerInput PlayerInput { get; private set; }
    public IPlayerMovement PlayerMovement { get; private set; }

    private StateMachine stateMachine;
    private Dictionary<PlayerState, IState> states;

    private bool isGrounded = false;
    private bool isFacingRight = true;

    void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
        PlayerInput = GetComponent<KeyboardInput>();

        PlayerMovement = new NormalMovement();
        PlayerMovement.Initialize(PlayerStats);

        states = new();
        states.Add(PlayerState.Idle, new IdleState(this));
        states.Add(PlayerState.Walk, new WalkState(this));

        stateMachine = new();
        stateMachine.Initialize(GetState(PlayerState.Idle));
    }

    void Update()
    {
        if (PlayerInput.GetMovementInput() > 0f && !isFacingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;
        }
        else if (PlayerInput.GetMovementInput() < 0f && isFacingRight)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;
        }

        stateMachine.Update();
    }

    public bool IsFacingRight() => isFacingRight;

    public bool IsGrounded() => isGrounded;

    public bool IsWalking() => PlayerInput.GetMovementInput() != 0 && isGrounded;

    public bool IsIdling() => PlayerInput.GetMovementInput() == 0 && isGrounded;

    public IState GetState(PlayerState state) => states[state];

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
