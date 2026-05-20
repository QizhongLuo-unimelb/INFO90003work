using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestMailPopupUI : MonoBehaviour
{
    public static TestMailPopupUI Instance { get; private set; }

    [Header("Template")]
    public RectTransform popupTemplate;

    [Header("Timing")]
    public float visibleSeconds = 3.2f;
    public float minimumInterval = 0.28f;
    public int maxVisiblePopups = 5;

    [Header("Placement")]
    public Vector2 horizontalRange = new Vector2(-520f, 520f);
    public Vector2 verticalRange = new Vector2(-260f, 280f);

    readonly List<RectTransform> activePopups = new List<RectTransform>();
    float nextAllowedTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (popupTemplate != null)
        {
            popupTemplate.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowSubject(string subject)
    {
        if (popupTemplate == null || Time.time < nextAllowedTime)
        {
            return;
        }

        nextAllowedTime = Time.time + minimumInterval;

        while (activePopups.Count >= maxVisiblePopups)
        {
            RectTransform oldest = activePopups[0];
            activePopups.RemoveAt(0);
            if (oldest != null)
            {
                Destroy(oldest.gameObject);
            }
        }

        RectTransform popup = Instantiate(popupTemplate, popupTemplate.parent);
        popup.gameObject.SetActive(true);
        popup.anchoredPosition = new Vector2(
            Random.Range(horizontalRange.x, horizontalRange.y),
            Random.Range(verticalRange.x, verticalRange.y));

        TMP_Text[] texts = popup.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text.name == "SubjectText")
            {
                text.text = subject;
                break;
            }
        }

        activePopups.Add(popup);
        StartCoroutine(FadeAndDestroy(popup));
    }

    System.Collections.IEnumerator FadeAndDestroy(RectTransform popup)
    {
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = popup.gameObject.AddComponent<CanvasGroup>();
        }

        float elapsed = 0f;
        while (elapsed < visibleSeconds)
        {
            elapsed += Time.deltaTime;
            float fadeStart = Mathf.Max(0f, visibleSeconds - 0.7f);
            canvasGroup.alpha = elapsed < fadeStart ? 1f : 1f - Mathf.InverseLerp(fadeStart, visibleSeconds, elapsed);
            yield return null;
        }

        activePopups.Remove(popup);
        if (popup != null)
        {
            Destroy(popup.gameObject);
        }
    }
}
