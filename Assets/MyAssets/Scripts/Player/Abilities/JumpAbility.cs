using UnityEngine;

public class JumpAbility : MovementAbility
{
    public override void TickAbility(float dt)
    {
        if (!Controller.HasBufferedJump)
        {
            return;
        }

        if (!Controller.IsGrounded && !Controller.CanCoyoteJump)
        {
            return;
        }

        Controller.ConsumeJumpBuffer();

        float jumpHeight = Stats.GetValue(StatType.JumpHeight);
        float gravity = Stats.GetValue(StatType.Gravity);
        Controller.SetVerticalVelocity(Mathf.Sqrt(jumpHeight * -2f * gravity));
        Controller.ExpireCoyote();
        Controller.NotifyJumped();
    }
}
