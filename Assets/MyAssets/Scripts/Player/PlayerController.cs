using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxMoveSpeed = 7f;
    public float acceleration = 28f;
    public float deceleration = 40f;
    public float airControlMultiplier = 0.45f;
    public float rotationSpeed = 14f;

    [Header("Jump and Gravity")]
    public float jumpHeight = 1.6f;
    public float gravity = -30f;
    public float groundedSnap = -2f;

    private CharacterController controller;
    private ThirdPersonCamera cameraController;
    private PlayerInputHandler input;

    private Vector3 planarVelocity;
    private float verticalVelocity;

    public Vector3 PlanarVelocity => planarVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputHandler>();

        if (Camera.main != null)
        {
            cameraController = Camera.main.GetComponent<ThirdPersonCamera>();
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        Vector3 desiredMoveDirection = GetDesiredMoveDirection();
        UpdatePlanarVelocity(desiredMoveDirection, dt);
        HandleJumpAndGravity(dt);
        ApplyMovement(dt);
        UpdateFacing(dt);
    }

    private Vector3 GetDesiredMoveDirection()
    {
        Vector2 moveInput = input.MoveInput;

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
        Vector3 targetPlanarVelocity = desiredMoveDirection * maxMoveSpeed;

        bool hasInput = desiredMoveDirection.sqrMagnitude > 0.0001f;
        float rate = hasInput ? acceleration : deceleration;

        if (!controller.isGrounded)
        {
            rate *= airControlMultiplier;
        }

        planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVelocity, rate * dt);
    }

    private void HandleJumpAndGravity(float dt)
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedSnap;
        }

        if (controller.isGrounded && input.ConsumeJumpPressed())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * dt;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * dt);
    }
}