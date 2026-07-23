using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Effects/Modify Countdown", fileName = "ModifyCountdownEffect")]
public class ModifyCountdownEffect : PowerupEffect
{
    public enum CountdownEffectKind
    {
        AddStartingTime,
        SlowCountdown,
    }

    public CountdownEffectKind kind = CountdownEffectKind.AddStartingTime;

    [Tooltip("Seconds added per level (AddStartingTime) or multiplier reduction per level, e.g. 0.1 = 10% slower (SlowCountdown).")]
    public float valuePerLevel = 5f;

    public override void Apply(PowerupContext context, int newLevel)
    {
        if (CountdownManager.Instance == null)
        {
            return;
        }

        if (kind == CountdownEffectKind.AddStartingTime)
        {
            CountdownManager.Instance.AddBonusStartingTime(valuePerLevel);
        }
        else
        {
            CountdownManager.Instance.AddCountdownSpeedMultiplier(-valuePerLevel);
        }
    }

    public override string GetEffectSummary(int level)
    {
        if (kind == CountdownEffectKind.AddStartingTime)
        {
            return $"+{valuePerLevel:0.#}s Starting Time";
        }

        return $"-{valuePerLevel * 100f:0.#}% Countdown Speed";
    }
}
