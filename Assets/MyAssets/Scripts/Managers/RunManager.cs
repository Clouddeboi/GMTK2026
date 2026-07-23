using System.Collections;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    [Tooltip("Player to reset. Auto-found in the scene if left empty.")]
    public PlayerController player;
    [Tooltip("Auto-found in the scene if left empty.")]
    public PlayerStateManager playerStateManager;

    [Tooltip("Spawn point to reset the player to. Falls back to PlayerSpawnPoint.Current if left empty.")]
    public Transform spawnPointOverride;

    [Tooltip("Seconds to wait after death before resetting the attempt.")]
    public float deathDelay = 1.5f;

    private Coroutine resetRoutine;
    private bool awaitingReset;

    private void Awake()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (playerStateManager == null && player != null)
        {
            playerStateManager = player.GetComponent<PlayerStateManager>();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += HandlePlayerDeath;
        GameEvents.OnPowerupSelected += HandlePowerupSelected;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
        GameEvents.OnPowerupSelected -= HandlePowerupSelected;
    }

    private void HandlePlayerDeath()
    {
        // Ignore duplicate death events that arrive while we're already mid-sequence
        // (e.g. a stray/late GameEvents.PlayerDeath() call) so the delay doesn't restart.
        if (awaitingReset)
        {
            return;
        }

        awaitingReset = true;

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
        }

        resetRoutine = StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);

        // Only hand off to the powerup selection UI if something is actually listening for
        // it. Otherwise (UI not set up in the scene yet) fall back to resetting immediately
        // instead of getting stuck waiting forever for a selection that will never come.
        if (GameEvents.OnPowerupSelectionRequested != null)
        {
            GameEvents.RequestPowerupSelection();
        }
        else
        {
            ResetRun();
        }
    }

    private void HandlePowerupSelected()
    {
        ResetRun();
    }

    public void ResetRun()
    {
        Transform spawnPoint = spawnPointOverride;
        if (spawnPoint == null && PlayerSpawnPoint.Current != null)
        {
            spawnPoint = PlayerSpawnPoint.Current.transform;
        }

        if (player != null && spawnPoint != null)
        {
            player.Teleport(spawnPoint.position, spawnPoint.rotation);
        }

        if (CountdownManager.Instance != null)
        {
            CountdownManager.Instance.ResetTimer();
        }

        if (playerStateManager != null)
        {
            playerStateManager.Revive();
        }

        awaitingReset = false;

        GameEvents.RunReset();
    }
}
