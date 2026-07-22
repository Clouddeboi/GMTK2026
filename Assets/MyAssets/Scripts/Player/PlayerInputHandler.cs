using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool IsUsingGamepad { get; private set; }

    private bool jumpQueued;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Keep jump event-based for queue semantics.
        inputActions.Player.Jump.started += OnJumpStarted;
        inputActions.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void OnDestroy()
    {
        inputActions.Player.Jump.started -= OnJumpStarted;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;
        inputActions.Dispose();
    }

    private void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        LookInput = inputActions.Player.Look.ReadValue<Vector2>();

        var lookControl = inputActions.Player.Look.activeControl;
        var moveControl = inputActions.Player.Move.activeControl;
        var device = lookControl != null ? lookControl.device : moveControl != null ? moveControl.device : null;
        if (device != null)
        {
            IsUsingGamepad = device is Gamepad;
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        jumpQueued = true;
        JumpHeld = true;
        IsUsingGamepad = ctx.control?.device is Gamepad;
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        JumpHeld = false;
    }

    public bool ConsumeJumpPressed()
    {
        if (!jumpQueued) return false;
        jumpQueued = false;
        return true;
    }
}