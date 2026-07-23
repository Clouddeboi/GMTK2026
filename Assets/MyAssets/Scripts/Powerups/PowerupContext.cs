public class PowerupContext
{
    public readonly PlayerController Player;

    // Resolved on demand rather than cached at construction time: Unity does not guarantee
    // Awake() order across different GameObjects, so PlayerController.Stats might not be
    // assigned yet if this context is built before the player's own Awake() runs.
    public PlayerStats Stats => Player != null ? Player.Stats : null;

    public PowerupContext(PlayerController player)
    {
        Player = player;
    }
}
