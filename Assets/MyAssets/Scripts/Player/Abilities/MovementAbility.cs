using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public abstract class MovementAbility : MonoBehaviour
{
    public virtual int TickPriority => 0;

    // Whether this ability is currently usable at all. Defaults to true (always-available
    // abilities like basic Jump). Unlockable abilities override this to reflect their own
    // unlock state (a stat threshold, or an internal flag) independent of the Unity
    // component's enabled flag, so a stat-only powerup can make an ability work even if it
    // was never explicitly "unlocked".
    public virtual bool IsUnlocked => true;

    // Called by UnlockAbilityEffect when a player selects an Ability Unlock powerup for this
    // ability. Default no-op for abilities that don't need explicit unlocking.
    public virtual void Unlock() { }

    protected PlayerController Controller { get; private set; }
    protected PlayerStats Stats { get; private set; }
    protected PlayerInputHandler Input { get; private set; }

    public virtual void InitializeAbility(PlayerController controller)
    {
        Controller = controller;
        Stats = controller.Stats;
        Input = controller.InputHandler;
    }

    public abstract void TickAbility(float dt);
}
