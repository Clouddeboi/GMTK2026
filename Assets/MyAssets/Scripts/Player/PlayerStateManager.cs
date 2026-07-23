using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerStateManager : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; } = PlayerState.Alive;

    public event Action<PlayerState> StateChanged;

    private PlayerController playerController;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        inputHandler = GetComponent<PlayerInputHandler>();
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
        SetState(PlayerState.Dead);
    }

    public void Kill()
    {
        if (CurrentState == PlayerState.Dead)
        {
            return;
        }

        GameEvents.PlayerDeath();
    }

    public void Revive()
    {
        SetState(PlayerState.Alive);
    }

    private void SetState(PlayerState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        CurrentState = newState;
        bool alive = newState == PlayerState.Alive;

        if (playerController != null)
        {
            playerController.enabled = alive;
        }

        if (inputHandler != null)
        {
            inputHandler.enabled = alive;
        }

        StateChanged?.Invoke(newState);
    }
}
