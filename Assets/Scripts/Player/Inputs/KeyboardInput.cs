using UnityEngine;

public class KeyboardInput : MonoBehaviour, IPlayerInput
{
    [Header("Keyboard Input Settings")]
    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [SerializeField]
    private KeyCode shieldKey = KeyCode.S;

    [SerializeField]
    private KeyCode dashKey = KeyCode.D;

    [SerializeField]
    private KeyCode skill1Key = KeyCode.Q;

    [SerializeField]
    private KeyCode skill2Key = KeyCode.W;

    [SerializeField]
    private KeyCode skill3Key = KeyCode.E;

    [SerializeField]
    private KeyCode skill4Key = KeyCode.R;

    [SerializeField]
    private KeyCode pauseKey = KeyCode.Escape;

    public float MovementInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsJumpHeld { get; private set; }
    public bool IsShieldPressed { get; private set; }
    public bool IsShieldHeld { get; private set; }
    public bool IsSkill1Pressed { get; private set; }
    public bool IsSkill2Pressed { get; private set; }
    public bool IsSkill3Pressed { get; private set; }
    public bool IsSkill4Pressed { get; private set; }
    public bool IsPausePressed { get; private set; }

    private void Awake()
    {
        MovementInput = 0f;
        IsJumpPressed = false;
        IsJumpHeld = false;
        IsShieldPressed = false;
        IsShieldHeld = false;
        IsSkill1Pressed = false;
        IsSkill2Pressed = false;
        IsSkill3Pressed = false;
        IsSkill4Pressed = false;
        IsPausePressed = false;
    }

    private void Update()
    {
        MovementInput = Input.GetAxisRaw("Horizontal");

        bool jumpPressed = Input.GetKeyDown(jumpKey);
        bool shieldPressed = Input.GetKeyDown(shieldKey);
        bool skill1Pressed = Input.GetKeyDown(skill1Key);
        bool skill2Pressed = Input.GetKeyDown(skill2Key);
        bool skill3Pressed = Input.GetKeyDown(skill3Key);
        bool skill4Pressed = Input.GetKeyDown(skill4Key);
        bool pausePressed = Input.GetKeyDown(pauseKey);

        IsJumpHeld = Input.GetKey(jumpKey);
        IsShieldHeld = Input.GetKey(shieldKey);

        IsJumpPressed = jumpPressed;
        IsShieldPressed = shieldPressed;
        IsSkill1Pressed = skill1Pressed;
        IsSkill2Pressed = skill2Pressed;
        IsSkill3Pressed = skill3Pressed;
        IsSkill4Pressed = skill4Pressed;
        IsPausePressed = pausePressed;
    }
}
