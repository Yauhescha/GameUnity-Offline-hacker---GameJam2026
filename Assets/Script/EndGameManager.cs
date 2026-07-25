using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    public static EndGameManager Instance { get; private set; }

    [Header("Failure screen")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text reasonText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("Success screen")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private GameObject successPanel;

    public bool IsFinished { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    public void Initialize(
        GameObject newEndGamePanel,
        TMP_Text newTitleText,
        TMP_Text newReasonText,
        Button newRestartButton,
        Button newQuitButton)
    {
        endGamePanel = newEndGamePanel;
        titleText = newTitleText;
        reasonText = newReasonText;
        restartButton = newRestartButton;
        quitButton = newQuitButton;

        endGamePanel.SetActive(false);

        restartButton.onClick.AddListener(Restart);
        quitButton.onClick.AddListener(Quit);

        CreateSuccessPanel();
    }

    public void EndFailure(string reason)
    {
        if (IsFinished)
            return;

        GameAudio.Instance?.PlayDefeatMusic();

        IsFinished = true;
        Shindows9UI.Instance?.ForceClose();

        titleText.text = "MISSION FAILED";
        titleText.color = new Color(1f, 0.28f, 0.28f);
        reasonText.text = reason;

        endGamePanel.SetActive(true);

        UnlockCursorAndPause();
    }

    public void EndSuccess(string message = "")
    {
        if (IsFinished)
            return;

        GameAudio.Instance?.PlayVictoryMusic();

        IsFinished = true;
        Shindows9UI.Instance?.ForceClose();

        if (successPanel != null)
            successPanel.SetActive(true);

        UnlockCursorAndPause();
    }

    private void UnlockCursorAndPause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private void CreateSuccessPanel()
    {
        if (endGamePanel == null || successPanel != null)
            return;

        Transform canvasTransform = endGamePanel.transform.parent;

        successPanel = CreateUIObject(
            "SuccessPanel",
            canvasTransform,
            typeof(Image)
        );

        RectTransform panelRect = successPanel.GetComponent<RectTransform>();
        StretchToFullScreen(panelRect);

        Image backgroundImage = successPanel.GetComponent<Image>();
        Sprite successSprite = Resources.Load<Sprite>("GameSuccess");

        if (successSprite != null)
        {
            backgroundImage.sprite = successSprite;
            backgroundImage.preserveAspect = false;
            backgroundImage.color = Color.white;
        }
        else
        {
            Debug.LogError(
                "GameSuccess.png not found. Expected path: Assets/Resources/GameSuccess.png",
                this
            );

            backgroundImage.color = new Color(0.05f, 0.12f, 0.10f, 1f);
        }

        CreateDarkOverlay(successPanel.transform);
        CreateSuccessText(successPanel.transform);
        CreateMainMenuButton(successPanel.transform);

        successPanel.SetActive(false);
    }

    private void CreateDarkOverlay(Transform parent)
    {
        GameObject overlay = CreateUIObject(
            "DarkOverlay",
            parent,
            typeof(Image)
        );

        RectTransform rect = overlay.GetComponent<RectTransform>();
        StretchToFullScreen(rect);

        Image image = overlay.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.42f);
    }

    private void CreateSuccessText(Transform parent)
    {
        GameObject textObject = CreateUIObject(
            "SuccessText",
            parent,
            typeof(TextMeshProUGUI)
        );

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.12f, 0.26f);
        rect.anchorMax = new Vector2(0.88f, 0.52f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TMP_Text text = textObject.GetComponent<TMP_Text>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = "You did it! And now you're rich enough not to work in retirement.";
        text.fontSize = 42f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;
        text.color = Color.white;

        text.outlineWidth = 0.2f;
        text.outlineColor = new Color(0f, 0f, 0f, 0.9f);
    }

    private void CreateMainMenuButton(Transform parent)
    {
        GameObject buttonObject = CreateUIObject(
            "MainMenuButton",
            parent,
            typeof(Image),
            typeof(Button)
        );

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.14f);
        rect.anchorMax = new Vector2(0.5f, 0.14f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(310f, 76f);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.12f, 0.82f, 0.48f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(LoadMainMenu);

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.82f, 1f, 0.89f);
        colors.pressedColor = new Color(0.52f, 0.9f, 0.67f);
        button.colors = colors;

        GameObject labelObject = CreateUIObject(
            "Label",
            buttonObject.transform,
            typeof(TextMeshProUGUI)
        );

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        StretchToFullScreen(labelRect);

        TMP_Text label = labelObject.GetComponent<TMP_Text>();
        label.font = TMP_Settings.defaultFontAsset;
        label.text = "MAIN MENU";
        label.fontSize = 28f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.02f, 0.12f, 0.06f);
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Quit()
    {
        Application.Quit();
    }

    private GameObject CreateUIObject(
        string objectName,
        Transform parent,
        params System.Type[] components)
    {
        GameObject obj = new GameObject(objectName, components);
        obj.transform.SetParent(parent, false);

        return obj;
    }

    private void StretchToFullScreen(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}