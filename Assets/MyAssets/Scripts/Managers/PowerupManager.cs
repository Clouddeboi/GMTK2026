using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }

    [Tooltip("Every powerup that can possibly be offered. Add new PowerupData assets here.")]
    public List<PowerupData> powerupPool = new List<PowerupData>();

    [Tooltip("Player to apply effects to. Auto-found in the scene if left empty.")]
    public PlayerController player;

    private readonly Dictionary<PowerupData, int> ownedLevels = new Dictionary<PowerupData, int>();

    private PowerupContext context;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Resolved on first use (rather than in Awake) so this works no matter what order Awake()
    // runs in relative to the player - FindAnyObjectByType always finds a fully-Awake object
    // by the time gameplay code (powerup selection) actually calls this.
    private PowerupContext Context
    {
        get
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController>();
            }

            if (context == null || context.Player != player)
            {
                context = new PowerupContext(player);
            }

            return context;
        }
    }

    public int GetLevel(PowerupData data)
    {
        return ownedLevels.TryGetValue(data, out int level) ? level : 0;
    }

    public bool IsAvailable(PowerupData data)
    {
        if (data == null || data.effect == null)
        {
            return false;
        }

        int currentLevel = GetLevel(data);
        if (currentLevel >= data.maxLevel)
        {
            return false;
        }

        return data.effect.IsAvailable(Context, currentLevel);
    }

    public List<PowerupData> GenerateChoices(int count = 3)
    {
        List<PowerupData> valid = new List<PowerupData>();
        foreach (PowerupData data in powerupPool)
        {
            if (IsAvailable(data))
            {
                valid.Add(data);
            }
        }

        List<PowerupData> choices = new List<PowerupData>();
        int pickCount = Mathf.Min(count, valid.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int index = Random.Range(0, valid.Count);
            choices.Add(valid[index]);
            valid.RemoveAt(index);
        }

        return choices;
    }

    public void ApplyPowerup(PowerupData data)
    {
        if (data == null || data.effect == null)
        {
            return;
        }

        int newLevel = GetLevel(data) + 1;
        ownedLevels[data] = newLevel;
        data.effect.Apply(Context, newLevel);
    }
}
