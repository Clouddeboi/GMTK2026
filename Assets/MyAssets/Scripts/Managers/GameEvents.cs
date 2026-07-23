using System;

public static class GameEvents
{

    public static Action OnPlayerDeath;

    public static Action OnPlayerVictory;

    public static Action OnRunReset;

    public static Action OnPowerupSelectionRequested;

    public static Action OnPowerupSelected;


    public static void PlayerDeath()
    {
        OnPlayerDeath?.Invoke();
    }

    public static void PlayerVictory()
    {
        OnPlayerVictory?.Invoke();
    }

    public static void RunReset()
    {
        OnRunReset?.Invoke();
    }

    public static void RequestPowerupSelection()
    {
        OnPowerupSelectionRequested?.Invoke();
    }

    public static void PowerupSelected()
    {
        OnPowerupSelected?.Invoke();
    }

}