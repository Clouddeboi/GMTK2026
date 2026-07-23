using System;
using UnityEngine;

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager Instance { get; private set; }

    [Tooltip("Seconds the countdown starts at for each attempt.")]
    public float startingTime = 60f;

    [Tooltip("Permanent bonus seconds added on top of startingTime, granted by powerups.")]
    public float bonusStartingTime = 0f;

    [Tooltip("Permanent multiplier applied to how fast the countdown ticks down. 1 = normal, <1 = slower.")]
    public float countdownSpeedMultiplier = 1f;

    public float TimeRemaining { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> TimeChanged;

    public event Action TimeExpired;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void Start()
    {
        StartTimer();
    }

    // Stops the countdown the moment the player dies for ANY reason (falling, enemy, etc.),
    // not just on timeout. Without this, the timer keeps running in the background after a
    // non-timeout death, eventually hits zero, and fires a second PlayerDeath() - which
    // restarts RunManager's death-delay sequence and delays the powerup selection.
    private void HandlePlayerDeath()
    {
        StopTimer();
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        TimeRemaining -= Time.deltaTime * countdownSpeedMultiplier;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsRunning = false;
            TimeChanged?.Invoke(TimeRemaining);
            TimeExpired?.Invoke();

            GameEvents.PlayerDeath();
            return;
        }

        TimeChanged?.Invoke(TimeRemaining);
    }

    public void StartTimer()
    {
        TimeRemaining = startingTime + bonusStartingTime;
        IsRunning = true;
        TimeChanged?.Invoke(TimeRemaining);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        StartTimer();
    }

    public void AddBonusStartingTime(float extraSeconds)
    {
        bonusStartingTime += extraSeconds;
    }

    public void AddCountdownSpeedMultiplier(float multiplierDelta)
    {
        countdownSpeedMultiplier = Mathf.Max(0.1f, countdownSpeedMultiplier + multiplierDelta);
    }
}
