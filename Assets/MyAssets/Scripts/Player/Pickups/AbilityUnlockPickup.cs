using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AbilityUnlockPickup : MonoBehaviour
{
    [Tooltip("The ability component type to unlock, matched by simple type name (e.g. \"DashAbility\").")]
    public string abilityTypeName = "DashAbility";
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        foreach (MovementAbility ability in player.GetComponents<MovementAbility>())
        {
            if (ability.GetType().Name == abilityTypeName)
            {
                ability.enabled = true;
                break;
            }
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
