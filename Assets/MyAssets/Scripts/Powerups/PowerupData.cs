using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Powerup Data", fileName = "NewPowerup")]
public class PowerupData : ScriptableObject
{
    public string powerupName = "New Powerup";

    [TextArea]
    public string description = "Description of what this powerup does.";

    public Sprite icon;

    public PowerupRarity rarity = PowerupRarity.Common;

    public PowerupType powerupType = PowerupType.StatUpgrade;

    [Tooltip("Highest level this powerup can reach. Ability unlocks should usually be 1.")]
    public int maxLevel = 5;

    [Tooltip("The behaviour applied when this powerup is selected.")]
    public PowerupEffect effect;

    public string GetDisplayTitle(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        return maxLevel <= 1 ? powerupName : $"{powerupName} {ToRoman(nextLevel)}";
    }

    public string GetEffectSummary(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        return effect != null ? effect.GetEffectSummary(nextLevel) : string.Empty;
    }

    private static string ToRoman(int value)
    {
        switch (value)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            case 8: return "VIII";
            case 9: return "IX";
            case 10: return "X";
            default: return value.ToString();
        }
    }
}
