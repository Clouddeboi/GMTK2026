using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StatBoostPickup : MonoBehaviour
{
    public StatType statType = StatType.MoveSpeed;
    public StatModifierType modifierType = StatModifierType.PercentAdditive;
    public float value = 0.2f;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats stats = other.GetComponent<PlayerStats>();
        if (stats == null)
        {
            return;
        }

        stats.AddModifier(statType, value, modifierType, source: this);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}
