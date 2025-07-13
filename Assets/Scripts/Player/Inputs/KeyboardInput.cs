using UnityEngine;

public class KeyboardInput : MonoBehaviour, IPlayerInput
{
    [Header("Keyboard Input Settings")]
    [SerializeField]
    private KeyCode attackKey = KeyCode.J;

    [SerializeField]
    private KeyCode jumpKey = KeyCode.Space;

    [SerializeField]
    private KeyCode chargeKey = KeyCode.K;

    [SerializeField]
    private KeyCode dashKey = KeyCode.L;

    [SerializeField]
    private KeyCode shieldKey = KeyCode.S;

    [SerializeField]
    private KeyCode pauseKey = KeyCode.Escape;

    public float GetMovementInput() => Input.GetAxisRaw("Horizontal");

    public bool IsJumpPressed() => Input.GetKeyDown(jumpKey);

    public bool IsJumpHeld() => Input.GetKey(jumpKey);

    public bool IsShieldPressed() => Input.GetKeyDown(shieldKey);

    public bool IsShieldHeld() => Input.GetKey(shieldKey);

    public bool IsAttackPressed() => Input.GetKeyDown(attackKey);

    public bool IsAttackHeld() => Input.GetKey(attackKey);

    public bool IsChargePressed() => Input.GetKeyDown(chargeKey);

    public bool IsChargeHeld() => Input.GetKey(chargeKey);

    public bool IsDashPressed() => Input.GetKeyDown(dashKey);

    public bool IsPausePressed() => Input.GetKeyDown(pauseKey);
}
