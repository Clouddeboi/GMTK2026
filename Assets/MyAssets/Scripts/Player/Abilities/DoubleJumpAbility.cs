using UnityEngine;

public class DoubleJumpAbility : MovementAbility
{
    [Tooltip("Air jumps granted the first time this ability is enabled, if ExtraAirJumps is still 0.")]
    public int defaultExtraAirJumps = 1;

    private int airJumpsUsed;

    // No longer gated by the component's enabled flag - any effect that raises ExtraAirJumps
    // above 0 (a dedicated unlock OR a plain stat-upgrade powerup) makes this ability work.
    public override bool IsUnlocked => Stats != null && Stats.GetValue(StatType.ExtraAirJumps) > 0f;

    public override void Unlock()
    {
        if (Stats != null && Stats.GetValue(StatType.ExtraAirJumps) <= 0f)
        {
            Stats.SetBaseValue(StatType.ExtraAirJumps, defaultExtraAirJumps);
        }
    }

    public override void TickAbility(float dt)
    {
        if (Controller.IsGrounded)
        {
            airJumpsUsed = 0;
            return;
        }

        if (Controller.CanCoyoteJump)
        {
            return;
        }

        if (!Controller.HasBufferedJump)
        {
            return;
        }

        int maxAirJumps = Mathf.RoundToInt(Stats.GetValue(StatType.ExtraAirJumps));
        if (airJumpsUsed >= maxAirJumps)
        {
            return;
        }

        Controller.ConsumeJumpBuffer();
        airJumpsUsed++;

        float jumpHeight = Stats.GetValue(StatType.JumpHeight);
        float gravity = Stats.GetValue(StatType.Gravity);
        Debug.Log($"[DoubleJumpAbility] Firing air jump #{airJumpsUsed} (jumpHeight={jumpHeight}, gravity={gravity})");
        Controller.SetVerticalVelocity(Mathf.Sqrt(jumpHeight * -2f * gravity));
        Controller.NotifyJumped();
    }
}
