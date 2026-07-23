using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    public static PlayerSpawnPoint Current { get; private set; }

    private void Awake()
    {
        Current = this;
    }

    public static void SetCurrent(PlayerSpawnPoint spawnPoint)
    {
        Current = spawnPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
}
