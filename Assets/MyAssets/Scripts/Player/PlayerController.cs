using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    [Header("Platforming Feel")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    public float groundedSnap = -2f;

    public PlayerStats Stats { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }

    private CharacterController controller;
    private ThirdPersonCamera cameraController;
    private readonly List<MovementAbility> abilities = new List<MovementAbility>();

    private Vector3 planarVelocity;
    private float verticalVelocity;
    private float coyoteTimer;
    private float jumpBufferTimer;

    public Vector3 PlanarVelocity => planarVelocity;
    public float VerticalVelocity => verticalVelocity;
    public bool IsGrounded { get; private set; }
    public bool CanCoyoteJump => coyoteTimer > 0f;
    public bool HasBufferedJump => jumpBufferTimer > 0f;

    public bool ExternalPlanarControl { get; set; }

    public event Action Jumped;
    public event Action Landed;
    public event Action SpinPerformed;
    public event Action DashPerformed;
    public event Action WallJumped;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Stats = GetComponent<PlayerStats>();

        if (Camera.main != null)
        {
            cameraController = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        GetComponents(abilities);
        abilities.Sort((a, b) => a.TickPriority.CompareTo(b.TickPriority));
        foreach (MovementAbility ability in abilities)
        {
            ability.InitializeAbility(this);
        }

        Debug.Log($"[PlayerController] Found {abilities.Count} MovementAbility component(s) on '{gameObject.name}':");
        foreach (MovementAbility ability in abilities)
        {
            Debug.Log($"[PlayerController]  - {ability.GetType().Name} (enabled={ability.enabled})");
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        UpdateGroundedState();
        UpdateJumpBuffer();

        Vector3 desiredMoveDirection = GetDesiredMoveDirection();
        if (!ExternalPlanarControl)
        {
            UpdatePlanarVelocity(desiredMoveDirection, dt);
        }

        TickAbilities(dt);

        ApplyGravity(dt);
        ApplyMovement(dt);
        UpdateFacing(dt);
    }

    private void TickAbilities(float dt)
    {
        foreach (MovementAbility ability in abilities)
        {
            ability.TickAbility(dt);
        }
    }

    private void UpdateGroundedState()
    {
        bool wasGrounded = IsGrounded;
        IsGrounded = controller.isGrounded;

        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
            if (!wasGrounded)
            {
                Landed?.Invoke();
            }
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void UpdateJumpBuffer()
    {
        if (InputHandler.JumpPressedThisFrame)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }

    private Vector3 GetDesiredMoveDirection()
    {
        Vector2 moveInput = InputHandler.MoveInput;

        Vector3 forward;
        Vector3 right;

        if (cameraController != null)
        {
            forward = cameraController.GetCameraForward();
            right = cameraController.GetCameraRight();
        }
        else
        {
            forward = transform.forward;
            right = transform.right;
        }

        Vector3 move = (forward * moveInput.y) + (right * moveInput.x);
        move.y = 0f;

        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        return move;
    }

    private void UpdatePlanarVelocity(Vector3 desiredMoveDirection, float dt)
    {
        float moveSpeed = Stats.GetValue(StatType.MoveSpeed);
        Vector3 targetPlanarVelocity = desiredMoveDirection * moveSpeed;

        bool hasInput = desiredMoveDirection.sqrMagnitude > 0.0001f;
        float rate = hasInput ? Stats.GetValue(StatType.Acceleration) : Stats.GetValue(StatType.Deceleration);

        if (!IsGrounded)
        {
            rate *= Stats.GetValue(StatType.AirControl);
        }

        planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVelocity, rate * dt);
    }

    private void ApplyGravity(float dt)
    {
        if (IsGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedSnap;
        }

        verticalVelocity += Stats.GetValue(StatType.Gravity) * dt;
    }

    private void ApplyMovement(float dt)
    {
        Vector3 velocity = planarVelocity + Vector3.up * verticalVelocity;
        CollisionFlags flags = controller.Move(velocity * dt);

        if ((flags & CollisionFlags.Below) != 0 && verticalVelocity < 0f)
        {
            verticalVelocity = groundedSnap;
        }
    }

    private void UpdateFacing(float dt)
    {
        Vector3 flatVelocity = planarVelocity;
        flatVelocity.y = 0f;

        if (flatVelocity.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flatVelocity.normalized, Vector3.up);

        if (ExternalPlanarControl)
        {
            transform.rotation = targetRotation;
            return;
        }

        float rotationSpeed = Stats.GetValue(StatType.RotationSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * dt);
    }

    public void SetVerticalVelocity(float value) => verticalVelocity = value;

    public void SetPlanarVelocity(Vector3 value) => planarVelocity = value;

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        controller.enabled = false;
        transform.SetPositionAndRotation(position, rotation);
        controller.enabled = true;

        SetPlanarVelocity(Vector3.zero);
        SetVerticalVelocity(0f);
    }

    public void ConsumeJumpBuffer() => jumpBufferTimer = 0f;

    public void ExpireCoyote() => coyoteTimer = 0f;

    public void NotifyJumped() => Jumped?.Invoke();

    public void NotifySpin() => SpinPerformed?.Invoke();

    public void NotifyDash() => DashPerformed?.Invoke();

    public void NotifyWallJump() => WallJumped?.Invoke();

    public Vector3 GetFacingDirection()
    {
        Vector3 moveDir = GetDesiredMoveDirection();
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            return moveDir.normalized;
        }

        Vector3 facing = transform.forward;
        facing.y = 0f;
        return facing.sqrMagnitude > 0.0001f ? facing.normalized : Vector3.forward;
    }

    public Vector3 GetInputMoveDirection()
    {
        return GetDesiredMoveDirection();
    }
}