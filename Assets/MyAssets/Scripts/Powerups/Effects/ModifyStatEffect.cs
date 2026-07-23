using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Effects/Modify Stat", fileName = "ModifyStatEffect")]
public class ModifyStatEffect : PowerupEffect
{
    public StatType statType = StatType.MoveSpeed;
    public StatModifierType modifierType = StatModifierType.PercentAdditive;

    [Tooltip("Amount applied EACH time this powerup is selected (i.e. per level gained).")]
    public float valuePerLevel = 0.15f;

    [Tooltip("Optional display name for the stat shown in the UI, e.g. \"Jump Height\".")]
    public string statDisplayName = "Stat";

    public override void Apply(PowerupContext context, int newLevel)
    {
        if (context.Stats == null)
        {
            return;
        }

        context.Stats.AddModifier(statType, valuePerLevel, modifierType, source: this);
    }

    public override string GetEffectSummary(int level)
    {
        if (modifierType == StatModifierType.PercentAdditive)
        {
            return $"+{valuePerLevel * 100f:0.#}% {statDisplayName}";
        }

        return $"+{valuePerLevel:0.#} {statDisplayName}";
    }
}
