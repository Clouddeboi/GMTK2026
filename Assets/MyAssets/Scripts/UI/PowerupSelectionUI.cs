using System.Collections.Generic;
using UnityEngine;

public class PowerupSelectionUI : MonoBehaviour
{
    [Tooltip("Root object to show/hide for the whole selection panel.")]
    public GameObject panelRoot;

    [Tooltip("Pre-placed card slots in the panel (usually 3).")]
    public List<PowerupChoiceCardUI> cardSlots = new List<PowerupChoiceCardUI>();

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPowerupSelectionRequested += ShowSelection;
    }

    private void OnDisable()
    {
        GameEvents.OnPowerupSelectionRequested -= ShowSelection;
    }

    private void ShowSelection()
    {
        if (PowerupManager.Instance == null)
        {
            return;
        }

        List<PowerupData> choices = PowerupManager.Instance.GenerateChoices(cardSlots.Count);

        for (int i = 0; i < cardSlots.Count; i++)
        {
            if (i < choices.Count)
            {
                cardSlots[i].gameObject.SetActive(true);
                int currentLevel = PowerupManager.Instance.GetLevel(choices[i]);
                cardSlots[i].Setup(choices[i], currentLevel, this);
            }
            else
            {
                cardSlots[i].gameObject.SetActive(false);
            }
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
    }

    public void ChoosePowerup(PowerupData data)
    {
        PowerupManager.Instance.ApplyPowerup(data);

        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        GameEvents.PowerupSelected();
    }
}
