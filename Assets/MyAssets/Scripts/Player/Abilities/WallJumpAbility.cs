using UnityEngine;

public class WallJumpAbility : MovementAbility
{
    public override int TickPriority => -10;

    public LayerMask wallMask = ~0;
    public float checkDistance = 0.6f;
    public float checkHeight = 0.6f;

    private bool unlocked;

    public override bool IsUnlocked => unlocked;

    public override void Unlock()
    {
        unlocked = true;
    }

    public override void TickAbility(float dt)
    {
        if (!unlocked)
        {
            return;
        }

        if (Controller.IsGrounded)
        {
            return;
        }

        if (!Controller.HasBufferedJump)
        {
            return;
        }

        Vector3 origin = Controller.transform.position + Vector3.up * checkHeight;
        Vector3 dir = Controller.GetFacingDirection();
        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = Controller.transform.forward;
        }

        if (!Physics.Raycast(origin, dir, out RaycastHit hit, checkDistance, wallMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        Controller.ConsumeJumpBuffer();

        float force = Stats.GetValue(StatType.WallJumpForce);
        Vector3 launchDirection = (hit.normal + Vector3.up).normalized;
        Debug.Log($"[WallJumpAbility] Firing wall jump off '{hit.collider.name}' (normal={hit.normal}, force={force})");

        Controller.SetPlanarVelocity(new Vector3(launchDirection.x, 0f, launchDirection.z) * force);
        Controller.SetVerticalVelocity(launchDirection.y * force);
        Controller.NotifyJumped();
    }
}
