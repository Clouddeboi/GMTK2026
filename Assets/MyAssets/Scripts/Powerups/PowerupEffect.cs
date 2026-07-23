using UnityEngine;

public abstract class PowerupEffect : ScriptableObject
{
    public abstract void Apply(PowerupContext context, int newLevel);

    public abstract string GetEffectSummary(int level);

    public virtual bool IsAvailable(PowerupContext context, int currentLevel)
    {
        return true;
    }
}
