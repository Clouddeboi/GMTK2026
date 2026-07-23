using UnityEngine;

public class SpinJumpAbility : MovementAbility
{
    private bool usedSinceGrounded;

    public override void TickAbility(float dt)
    {
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
