using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Serializable]
    public struct StatEntry
    {
        public StatType type;
        public float baseValue;
    }

    [Tooltip("Starting values for every stat. Only stats listed here are available at runtime.")]
    public List<StatEntry> baseStats = new List<StatEntry>
    {
        new StatEntry { type = StatType.MoveSpeed, baseValue = 7f },
        new StatEntry { type = StatType.Acceleration, baseValue = 28f },
        new StatEntry { type = StatType.Deceleration, baseValue = 40f },
        new StatEntry { type = StatType.AirControl, baseValue = 0.45f },
        new StatEntry { type = StatType.RotationSpeed, baseValue = 14f },

        new StatEntry { type = StatType.JumpHeight, baseValue = 1.6f },
        new StatEntry { type = StatType.Gravity, baseValue = -30f },

        new StatEntry { type = StatType.ExtraAirJumps, baseValue = 0f },

        new StatEntry { type = StatType.DashCount, baseValue = 0f },
        new StatEntry { type = StatType.DashDistance, baseValue = 6f },
        new StatEntry { type = StatType.DashDuration, baseValue = 0.18f },
        new StatEntry { type = StatType.DashCooldown, baseValue = 0.5f },

        new StatEntry { type = StatType.WallJumpForce, baseValue = 9f },

        new StatEntry { type = StatType.SpinJumpVelocity, baseValue = 8f },
    };

    private readonly Dictionary<StatType, StatValue> stats = new Dictionary<StatType, StatValue>();

    private void Awake()
    {
        foreach (StatEntry entry in baseStats)
        {
            stats[entry.type] = new StatValue(entry.baseValue);
        }
    }

    public float GetValue(StatType type)
    {
        if (stats.TryGetValue(type, out StatValue value))
        {
            return value.Value;
        }

        Debug.LogWarning($"PlayerStats: stat '{type}' was requested but not registered in baseStats.");
        return 0f;
    }

    public void SetBaseValue(StatType type, float baseValue)
    {
        if (!stats.TryGetValue(type, out StatValue value))
        {
            value = new StatValue(baseValue);
            stats[type] = value;
            return;
        }

        value.BaseValue = baseValue;
    }

    public StatModifier AddModifier(StatType type, float value, StatModifierType modType, object source = null)
    {
        if (!stats.TryGetValue(type, out StatValue statValue))
        {
            statValue = new StatValue(0f);
            stats[type] = statValue;
        }

        var modifier = new StatModifier(value, modType, source);
        statValue.AddModifier(modifier);
        return modifier;
    }

    public void RemoveModifier(StatType type, StatModifier modifier)
    {
        if (stats.TryGetValue(type, out StatValue statValue))
        {
            statValue.RemoveModifier(modifier);
        }
    }

    public void RemoveModifiersFromSource(StatType type, object source)
    {
        if (stats.TryGetValue(type, out StatValue statValue))
        {
            statValue.RemoveModifiersFromSource(source);
        }
    }

    public void RemoveModifiersFromSourceAll(object source)
    {
        foreach (StatValue statValue in stats.Values)
        {
            statValue.RemoveModifiersFromSource(source);
        }
    }
}
