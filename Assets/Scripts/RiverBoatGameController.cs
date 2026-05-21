using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RiverBoatGameController : MonoBehaviour
{
    [Header("Scene References")]
    public Transform boat;
    public Transform goal;
    public Text statusText;
    public Text timerText;
    public RiverBoatShoreTrigger shoreTrigger;

    [Header("Route")]
    public float travelSeconds = 30f;
    public Vector3 startPosition = new Vector3(-12f, 0.65f, 0f);
    public Vector3 goalPosition = new Vector3(12f, 0.65f, 0f);

    [Header("Boat Wobble")]
    public float startSwayAmplitude = 0.15f;
    public float endSwayAmplitude = 2.8f;
    public float distractionReachSeconds = 15f;
    public float swayFrequency = 0.25f;
    public float maxRollAngle = 24f;
    public float maxYawAngle = 12f;

    [Header("Finish")]
    public string returnSceneName = "Main game";
    public float returnDelay = 2f;

    float elapsed;
    float finishElapsed;
    bool finished;

    void Awake()
    {
        if (shoreTrigger != null)
        {
            shoreTrigger.controller = this;
        }
    }

    void Start()
    {
        if (boat != null)
        {
            boat.position = startPosition;
        }

        if (goal != null)
        {
            goal.position = goalPosition;
        }

        SetStatus("");
        UpdateTimer();
    }

    void Update()
    {
        if (finished)
        {
            finishElapsed += Time.deltaTime;
            if (finishElapsed >= returnDelay)
            {
                SceneManager.LoadScene(returnSceneName);
            }

            return;
        }

        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, travelSeconds));
        float x = Mathf.Lerp(startPosition.x, goalPosition.x, progress);

        float distractionProgress = Mathf.Clamp01(elapsed / Mathf.Max(0.01f, distractionReachSeconds));
        float amplitude = Mathf.Lerp(startSwayAmplitude, endSwayAmplitude, distractionProgress);
        float sway = Mathf.Sin(elapsed * swayFrequency * Mathf.PI * 2f) * amplitude;
        float roll = -Mathf.Sin(elapsed * swayFrequency * Mathf.PI * 2f) * maxRollAngle * distractionProgress;
        float yaw = Mathf.Cos(elapsed * swayFrequency * Mathf.PI * 2f) * maxYawAngle * distractionProgress;

        if (boat != null)
        {
            boat.position = new Vector3(x, startPosition.y, startPosition.z + sway);
            boat.rotation = Quaternion.Euler(0f, yaw, roll);
        }

        UpdateTimer();

        if (progress >= 1f)
        {
            Finish("success");
        }
    }

    public void ShoreTouched()
    {
        if (finished)
        {
            return;
        }

        Finish("distraction");
    }

    void Finish(string message)
    {
        finished = true;
        SetStatus(message);
    }

    void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }

    void UpdateTimer()
    {
        if (timerText == null)
        {
            return;
        }

        float remaining = Mathf.Max(0f, travelSeconds - elapsed);
        timerText.text = Mathf.CeilToInt(remaining) + "s";
    }
}
