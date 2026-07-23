using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerStateManager stateManager = other.GetComponentInParent<PlayerStateManager>();
        if (stateManager != null)
        {
            stateManager.Kill();
        }
    }
}
