using UnityEngine;

public enum NotificationKind
{
    Auto,
    Email,
    Game,
    Shopping,
    System
}

public class StepNode : MonoBehaviour
{
    [Header("Notification UI")]
    public NotificationKind notificationKind = NotificationKind.Auto;
    public string notificationTitle;

    [Header("Text shown when player reaches this node")]
    [TextArea(3, 6)]
    public string nodeMessage;
}
