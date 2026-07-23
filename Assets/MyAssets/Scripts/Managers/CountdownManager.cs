using System;
using UnityEngine;

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager Instance { get; private set; }

    [Tooltip("Seconds the countdown starts at for each attempt.")]
    public float startingTime = 60f;

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

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        TimeRemaining -= Time.deltaTime;

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
        TimeRemaining = startingTime;
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
}
