using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupChoiceCardUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text levelText;
    public Image iconImage;
    public Button chooseButton;

    private PowerupData data;
    private PowerupSelectionUI owner;

    public void Setup(PowerupData powerupData, int currentLevel, PowerupSelectionUI selectionOwner)
    {
        data = powerupData;
        owner = selectionOwner;

        if (titleText != null)
        {
            titleText.text = data.GetDisplayTitle(currentLevel);
        }

        if (descriptionText != null)
        {
            descriptionText.text = string.IsNullOrEmpty(data.description)
                ? data.GetEffectSummary(currentLevel)
                : $"{data.description}\n{data.GetEffectSummary(currentLevel)}";
        }

        if (levelText != null)
        {
            levelText.text = currentLevel <= 0
                ? $"LOCKED -> LEVEL 1"
                : $"{currentLevel}/{data.maxLevel}";
        }

        if (iconImage != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = data.icon != null;
        }

        if (chooseButton != null)
        {
            chooseButton.onClick.RemoveAllListeners();
            chooseButton.onClick.AddListener(HandleChosen);
        }
    }

    private void HandleChosen()
    {
        owner.ChoosePowerup(data);
    }
}
