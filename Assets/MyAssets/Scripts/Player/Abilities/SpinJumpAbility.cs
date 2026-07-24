using UnityEngine;

public class SpinJumpAbility : MovementAbility
{
    [Tooltip("If true, this ability is unlocked as soon as the game starts (no pickup needed). Useful for testing.")]
    public bool startUnlocked = false;

    private bool usedSinceGrounded;
    private bool unlocked;

    public override bool IsUnlocked => unlocked;

    public override void Unlock()
    {
        unlocked = true;
    }

    public override void InitializeAbility(PlayerController controller)
    {
        base.InitializeAbility(controller);
        if (startUnlocked)
        {
            Unlock();
        }
    }

    public override void TickAbility(float dt)
    {
        if (!unlocked)
        {
            return;
        }

        if (Controller.IsGrounded)
        {
            usedSinceGrounded = false;
            return;
        }

        if (usedSinceGrounded)
        {
            return;
        }

        if (!Input.SpinPressedThisFrame)
        {
            return;
        }

        usedSinceGrounded = true;

        float spinVelocity = Stats.GetValue(StatType.SpinJumpVelocity);
        Debug.Log($"[SpinJumpAbility] Firing spin jump (spinVelocity={spinVelocity})");
        Controller.SetVerticalVelocity(spinVelocity);
        Controller.NotifyJumped();
        Controller.NotifySpin();
    }
}
