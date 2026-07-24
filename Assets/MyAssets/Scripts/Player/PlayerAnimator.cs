using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    [Tooltip("Animator to drive. Defaults to one found on this object or its children.")]
    public Animator animator;

    [Header("Continuous Parameter Names")]
    public string speedParam = "Speed";
    public string verticalVelocityParam = "VerticalVelocity";
    public string groundedParam = "IsGrounded";
    public string isDashingParam = "IsDashing";

    [Header("Trigger Parameter Names")]
    public string jumpTrigger = "Jump";
    public string landTrigger = "Land";
    public string dashTrigger = "Dash";
    public string spinTrigger = "Spin";
    public string wallJumpTrigger = "WallJump";
    public string deathTrigger = "Death";
    public string victoryTrigger = "Victory";

    private PlayerController controller;
    private DashAbility dashAbility;

    private int speedHash;
    private int verticalVelocityHash;
    private int groundedHash;
    private int isDashingHash;
    private int jumpHash;
    private int landHash;
    private int dashHash;
    private int spinHash;
    private int wallJumpHash;
    private int deathHash;
    private int victoryHash;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        dashAbility = GetComponent<DashAbility>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        speedHash = Animator.StringToHash(speedParam);
        verticalVelocityHash = Animator.StringToHash(verticalVelocityParam);
        groundedHash = Animator.StringToHash(groundedParam);
        isDashingHash = Animator.StringToHash(isDashingParam);
        jumpHash = Animator.StringToHash(jumpTrigger);
        landHash = Animator.StringToHash(landTrigger);
        dashHash = Animator.StringToHash(dashTrigger);
        spinHash = Animator.StringToHash(spinTrigger);
        wallJumpHash = Animator.StringToHash(wallJumpTrigger);
        deathHash = Animator.StringToHash(deathTrigger);
        victoryHash = Animator.StringToHash(victoryTrigger);
    }

    private void OnEnable()
    {
        controller.Jumped += HandleJumped;
        controller.Landed += HandleLanded;
        controller.SpinPerformed += HandleSpin;
        controller.DashPerformed += HandleDash;
        controller.WallJumped += HandleWallJump;
        GameEvents.OnPlayerDeath += HandleDeath;
        GameEvents.OnPlayerVictory += HandleVictory;
    }

    private void OnDisable()
    {
        controller.Jumped -= HandleJumped;
        controller.Landed -= HandleLanded;
        controller.SpinPerformed -= HandleSpin;
        controller.DashPerformed -= HandleDash;
        controller.WallJumped -= HandleWallJump;
        GameEvents.OnPlayerDeath -= HandleDeath;
        GameEvents.OnPlayerVictory -= HandleVictory;
    }

    private void Update()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(speedHash, controller.PlanarVelocity.magnitude);
        animator.SetFloat(verticalVelocityHash, controller.VerticalVelocity);
        animator.SetBool(groundedHash, controller.IsGrounded);

        if (dashAbility != null)
        {
            animator.SetBool(isDashingHash, dashAbility.IsDashing);
        }
    }

    private void HandleJumped()
    {
        if (animator != null)
        {
            animator.SetTrigger(jumpHash);
        }
    }

    private void HandleLanded()
    {
        if (animator != null)
        {
            animator.SetTrigger(landHash);
        }
    }

    private void HandleSpin()
    {
        if (animator != null)
        {
            animator.SetTrigger(spinHash);
        }
    }

    private void HandleDash()
    {
        if (animator != null)
        {
            animator.SetTrigger(dashHash);
        }
    }

    private void HandleWallJump()
    {
        if (animator != null)
        {
            animator.SetTrigger(wallJumpHash);
        }
    }

    private void HandleDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger(deathHash);
        }
    }

    private void HandleVictory()
    {
        if (animator != null)
        {
            animator.SetTrigger(victoryHash);
        }
    }
}
