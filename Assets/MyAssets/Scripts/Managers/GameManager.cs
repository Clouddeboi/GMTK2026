using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        SetGameState(GameState.Playing);
    }


    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log($"Game State Changed: {CurrentState}");
    }


    public void PlayerDied()
    {
        SetGameState(GameState.Dead);

        Debug.Log("Player died");
    }


    public void PlayerWon()
    {
        SetGameState(GameState.Victory);

        Debug.Log("Player escaped!");
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += PlayerDied;
        GameEvents.OnPlayerVictory += PlayerWon;
    }


    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= PlayerDied;
        GameEvents.OnPlayerVictory -= PlayerWon;
}
}