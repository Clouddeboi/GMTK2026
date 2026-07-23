using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Effects/Unlock Ability", fileName = "UnlockAbilityEffect")]
public class UnlockAbilityEffect : PowerupEffect
{
    [Tooltip("Type name of the MovementAbility component to enable, e.g. \"DashAbility\".")]
    public string abilityTypeName;

    [Tooltip("Display name used in the UI summary, e.g. \"Double Jump\".")]
    public string abilityDisplayName;

    public override void Apply(PowerupContext context, int newLevel)
    {
        if (context.Player == null)
        {
            return;
        }

        foreach (MovementAbility ability in context.Player.GetComponents<MovementAbility>())
        {
            if (ability.GetType().Name == abilityTypeName)
            {
                ability.enabled = true;
                ability.Unlock();
                return;
            }
        }

        Debug.LogWarning($"UnlockAbilityEffect: no MovementAbility of type '{abilityTypeName}' found on player.");
    }

    public override string GetEffectSummary(int level)
    {
        return $"Unlocks {abilityDisplayName}";
    }

    public override bool IsAvailable(PowerupContext context, int currentLevel)
    {
        if (context.Player == null)
        {
            return true;
        }

        foreach (MovementAbility ability in context.Player.GetComponents<MovementAbility>())
        {
            if (ability.GetType().Name == abilityTypeName)
            {
                return !ability.IsUnlocked;
            }
        }

        return true;
    }
}
