using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneMinecraftUI : MonoBehaviour
{
    public TMP_Text summaryText;
    public TMP_Text timerText;
    public ScrollRect summaryScrollRect;

    public void SetSummary(string value)
    {
        if (summaryText != null)
        {
            summaryText.text = value;
        }

        if (summaryScrollRect != null)
        {
            summaryScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    public void UpdateDisplay(float elapsedSeconds, float totalSeconds)
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(Mathf.Max(0f, totalSeconds - elapsedSeconds)).ToString("00") + "s";
        }

        if (summaryScrollRect == null)
        {
            return;
        }

        float scrollDuration = Mathf.Max(1f, totalSeconds * 0.55f);
        float progress = Mathf.Clamp01(elapsedSeconds / scrollDuration);
        summaryScrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, progress);
    }
}
