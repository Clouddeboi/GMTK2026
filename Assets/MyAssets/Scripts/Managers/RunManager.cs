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
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
        }

        resetRoutine = StartCoroutine(ResetAfterDelay());
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
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

        GameEvents.RunReset();
    }
}
