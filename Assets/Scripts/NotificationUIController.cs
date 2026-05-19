using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUIController : MonoBehaviour
{
    const float PanelWidth = 960f;
    const float PanelHeight = 200f;
    const float IconSize = 62f;
    const float LeftPadding = 34f;
    const float RightPadding = 34f;
    const float TextLeft = 120f;
    const float TopPadding = 28f;
    const float TitleHeight = 34f;
    const float MessageTop = 72f;
    const float MessageHeight = 104f;
    const float TimeWidth = 96f;

    [Header("UI References")]
    public RectTransform notificationPanel;
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text messageText;
    public TMP_Text timeText;

    [Header("Icons")]
    public Sprite emailIcon;
    public Sprite gameIcon;
    public Sprite shoppingIcon;
    public Sprite defaultIcon;

    void Awake()
    {
        ApplyLayout();
    }

    void OnValidate()
    {
        ApplyLayout();
    }

    public void Show(StepNode node)
    {
        if (node == null)
        {
            return;
        }

        NotificationKind kind = ResolveKind(node);
        string title = string.IsNullOrWhiteSpace(node.notificationTitle)
            ? GetDefaultTitle(kind)
            : node.notificationTitle;

        ShowNotification(title, CleanMessage(node.nodeMessage), kind);
    }

    public void ShowSystemMessage(string title, string message)
    {
        ShowNotification(title, message, NotificationKind.System);
    }

    void ShowNotification(string title, string message, NotificationKind kind)
    {
        ApplyLayout();

        if (notificationPanel != null)
        {
            notificationPanel.gameObject.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (timeText != null)
        {
            timeText.text = "now";
        }

        if (iconImage != null)
        {
            iconImage.sprite = GetIcon(kind);
            iconImage.enabled = iconImage.sprite != null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
    }

    NotificationKind ResolveKind(StepNode node)
    {
        if (node.notificationKind != NotificationKind.Auto)
        {
            return node.notificationKind;
        }

        string source = (node.name + " " + node.nodeMessage).ToLowerInvariant();

        if (source.Contains("email") || source.Contains("gmail"))
        {
            return NotificationKind.Email;
        }

        if (source.Contains("shopping") || source.Contains("shop"))
        {
            return NotificationKind.Shopping;
        }

        if (source.Contains("game") || source.Contains("login") || source.Contains("checked in"))
        {
            return NotificationKind.Game;
        }

        return NotificationKind.System;
    }

    string GetDefaultTitle(NotificationKind kind)
    {
        switch (kind)
        {
            case NotificationKind.Email:
                return "Outlook";
            case NotificationKind.Game:
                return "Game Center";
            case NotificationKind.Shopping:
                return "Shopping";
            default:
                return "Notification";
        }
    }

    string CleanMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return "";
        }

        string trimmed = message.TrimStart();

        if (!trimmed.StartsWith("["))
        {
            return message;
        }

        int endOfCue = trimmed.IndexOf(']');
        if (endOfCue < 0)
        {
            return message;
        }

        return trimmed.Substring(endOfCue + 1).TrimStart('\r', '\n', ' ');
    }

    Sprite GetIcon(NotificationKind kind)
    {
        switch (kind)
        {
            case NotificationKind.Email:
                return emailIcon != null ? emailIcon : defaultIcon;
            case NotificationKind.Game:
                return gameIcon != null ? gameIcon : defaultIcon;
            case NotificationKind.Shopping:
                return shoppingIcon != null ? shoppingIcon : defaultIcon;
            default:
                return defaultIcon;
        }
    }

    void ApplyLayout()
    {
        if (notificationPanel != null)
        {
            notificationPanel.anchorMin = new Vector2(0.5f, 1f);
            notificationPanel.anchorMax = new Vector2(0.5f, 1f);
            notificationPanel.pivot = new Vector2(0.5f, 1f);
            notificationPanel.anchoredPosition = new Vector2(0f, -32f);
            notificationPanel.sizeDelta = new Vector2(PanelWidth, PanelHeight);
        }

        RectTransform iconRect = iconImage != null ? iconImage.rectTransform : null;
        if (iconRect != null)
        {
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconRect.anchorMin = new Vector2(0f, 1f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 1f);
            iconRect.anchoredPosition = new Vector2(LeftPadding, -56f);
            iconRect.sizeDelta = new Vector2(IconSize, IconSize);
        }

        RectTransform timeRect = timeText != null ? timeText.rectTransform : null;
        if (timeRect != null)
        {
            timeRect.anchorMin = new Vector2(1f, 1f);
            timeRect.anchorMax = new Vector2(1f, 1f);
            timeRect.pivot = new Vector2(1f, 1f);
            timeRect.anchoredPosition = new Vector2(-RightPadding, -TopPadding);
            timeRect.sizeDelta = new Vector2(TimeWidth, 28f);
        }

        RectTransform titleRect = titleText != null ? titleText.rectTransform : null;
        if (titleRect != null)
        {
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(TextLeft, -TopPadding);
            titleRect.sizeDelta = new Vector2(PanelWidth - TextLeft - RightPadding - TimeWidth - 12f, TitleHeight);
        }

        RectTransform messageRect = messageText != null ? messageText.rectTransform : null;
        if (messageRect != null)
        {
            messageRect.anchorMin = new Vector2(0f, 1f);
            messageRect.anchorMax = new Vector2(0f, 1f);
            messageRect.pivot = new Vector2(0f, 1f);
            messageRect.anchoredPosition = new Vector2(TextLeft, -MessageTop);
            messageRect.sizeDelta = new Vector2(PanelWidth - TextLeft - RightPadding, MessageHeight);
        }

        ConfigureText(titleText, TextAlignmentOptions.Left, TextWrappingModes.NoWrap, 21f, 0f);
        ConfigureText(messageText, TextAlignmentOptions.TopLeft, TextWrappingModes.Normal, 25f, -7f);
        ConfigureText(timeText, TextAlignmentOptions.Right, TextWrappingModes.NoWrap, 17f, 0f);
    }

    void ConfigureText(TMP_Text text, TextAlignmentOptions alignment, TextWrappingModes wrapping, float fontSize, float lineSpacing)
    {
        if (text == null)
        {
            return;
        }

        text.alignment = alignment;
        text.textWrappingMode = wrapping;
        text.fontSize = fontSize;
        text.lineSpacing = lineSpacing;
        text.margin = Vector4.zero;
        text.raycastTarget = false;
    }
}
