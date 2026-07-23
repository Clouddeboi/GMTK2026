using UnityEngine;

public class DashAbility : MovementAbility
{
    [Tooltip("Dash charges granted the first time this ability is enabled, if DashCount is still 0.")]
    public int defaultDashCount = 1;

    private float dashTimeRemaining;
    private float cooldownRemaining;
    private int dashesUsedSinceGrounded;
    private Vector3 dashDirection;

    // No longer gated by the component's enabled flag - any effect that raises DashCount above
    // 0 (a dedicated unlock OR a plain stat-upgrade powerup) makes this ability work.
    public override bool IsUnlocked => Stats != null && Stats.GetValue(StatType.DashCount) > 0f;

    public override void Unlock()
    {
        if (Stats != null && Stats.GetValue(StatType.DashCount) <= 0f)
        {
            Stats.SetBaseValue(StatType.DashCount, defaultDashCount);
        }
    }

    public override void TickAbility(float dt)
    {
        if (Controller.IsGrounded)
        {
            dashesUsedSinceGrounded = 0;
        }

        if (cooldownRemaining > 0f)
        {
            cooldownRemaining -= dt;
        }

        if (dashTimeRemaining > 0f)
        {
            dashTimeRemaining -= dt;

            Vector3 steerDirection = Controller.GetInputMoveDirection();
            if (steerDirection.sqrMagnitude > 0.0001f)
            {
                dashDirection = steerDirection.normalized;
            }

            float duration = Mathf.Max(0.01f, Stats.GetValue(StatType.DashDuration));
            float distance = Stats.GetValue(StatType.DashDistance);
            float speed = distance / duration;

            Controller.ExternalPlanarControl = true;
            Controller.SetPlanarVelocity(dashDirection * speed);

            if (dashTimeRemaining <= 0f)
            {
                Controller.ExternalPlanarControl = false;
                float maxMoveSpeed = Stats.GetValue(StatType.MoveSpeed);
                Vector3 exitVelocity = Controller.PlanarVelocity;
                if (exitVelocity.magnitude > maxMoveSpeed)
                {
                    Controller.SetPlanarVelocity(exitVelocity.normalized * maxMoveSpeed);
                }
            }
            return;
        }

        Controller.ExternalPlanarControl = false;

        int maxDashes = Mathf.RoundToInt(Stats.GetValue(StatType.DashCount));
        if (maxDashes <= 0)
        {
            return;
        }

        if (!Input.DashPressedThisFrame)
        {
            return;
        }

        if (dashesUsedSinceGrounded >= maxDashes || cooldownRemaining > 0f)
        {
            return;
        }

        dashDirection = Controller.GetFacingDirection();
        dashTimeRemaining = Stats.GetValue(StatType.DashDuration);
        cooldownRemaining = Stats.GetValue(StatType.DashCooldown);
        dashesUsedSinceGrounded++;
        Debug.Log($"[DashAbility] Firing dash #{dashesUsedSinceGrounded} direction={dashDirection} duration={dashTimeRemaining}");
    }
}
