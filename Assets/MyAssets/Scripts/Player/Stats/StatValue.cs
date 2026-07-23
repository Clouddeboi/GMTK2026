using System.Collections.Generic;

public class StatValue
{
    public float BaseValue;

    private readonly List<StatModifier> modifiers = new List<StatModifier>();

    public StatValue(float baseValue)
    {
        BaseValue = baseValue;
    }

    public float Value
    {
        get
        {
            float flatSum = 0f;
            float percentSum = 0f;

            for (int i = 0; i < modifiers.Count; i++)
            {
                StatModifier mod = modifiers[i];
                if (mod.Type == StatModifierType.Flat)
                {
                    flatSum += mod.Value;
                }
                else
                {
                    percentSum += mod.Value;
                }
            }

            return (BaseValue + flatSum) * (1f + percentSum);
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public bool RemoveModifier(StatModifier modifier)
    {
        return modifiers.Remove(modifier);
    }

    public void RemoveModifiersFromSource(object source)
    {
        modifiers.RemoveAll(m => m.Source == source);
    }
}
