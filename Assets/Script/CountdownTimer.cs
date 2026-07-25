using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField, Min(1)] private float startSeconds = 180f;
    [SerializeField] private TMP_Text timerText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new(1f, 0.75f, 0.20f);
    [SerializeField] private Color dangerColor = new(1f, 0.25f, 0.25f);

    private float remainingSeconds;
    private bool isRunning = true;

    public float RemainingSeconds => remainingSeconds;

    public static CountdownTimer Instance { get; private set; }

    public string FormattedTime
    {
        get
        {
            int seconds = Mathf.CeilToInt(remainingSeconds);
            return $"{seconds / 60:00}:{seconds % 60:00}";
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(TMP_Text newTimerText, float newStartSeconds)
    {
        timerText = newTimerText;
        startSeconds = newStartSeconds;
    }

    private void Start()
    {
        remainingSeconds = startSeconds;
        UpdateText();
    }

    private void Update()
    {
        if (!isRunning || EndGameManager.Instance.IsFinished)
            return;

        remainingSeconds -= Time.deltaTime;

        if (remainingSeconds <= 0f)
        {
            remainingSeconds = 0f;
            isRunning = false;
            UpdateText();

            EndGameManager.Instance.EndFailure(
                "Lunch break is over. The employees returned."
            );

            return;
        }

        UpdateText();
    }

    private void UpdateText()
    {
        int seconds = Mathf.CeilToInt(remainingSeconds);
        int minutes = seconds / 60;
        int secondsPart = seconds % 60;

        timerText.text = $"{minutes:00}:{secondsPart:00}";

        if (remainingSeconds <= 30f)
            timerText.color = dangerColor;
        else if (remainingSeconds <= 60f)
            timerText.color = warningColor;
        else
            timerText.color = normalColor;
    }
}