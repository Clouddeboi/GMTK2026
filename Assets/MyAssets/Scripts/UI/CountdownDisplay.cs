using TMPro;
using UnityEngine;

public class CountdownDisplay : MonoBehaviour
{
    public TMP_Text timerText;

    private void Start()
    {
        if (CountdownManager.Instance != null)
        {
            CountdownManager.Instance.TimeChanged += UpdateDisplay;
            UpdateDisplay(CountdownManager.Instance.TimeRemaining);
        }
    }

    private void OnDestroy()
    {
        if (CountdownManager.Instance != null)
        {
            CountdownManager.Instance.TimeChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay(float timeRemaining)
    {
        if (timerText == null)
        {
            return;
        }

        float clamped = Mathf.Max(0f, timeRemaining);
        int minutes = Mathf.FloorToInt(clamped / 60f);
        int seconds = Mathf.FloorToInt(clamped % 60f);
        int milliseconds = Mathf.FloorToInt((clamped * 1000f) % 1000f);

        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
