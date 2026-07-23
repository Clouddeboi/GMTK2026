using System;

public static class GameEvents
{

    public static Action OnPlayerDeath;

    public static Action OnPlayerVictory;

    public static Action OnRunReset;


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

}