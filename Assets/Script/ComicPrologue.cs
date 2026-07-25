using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class ComicFrame
{
    public Sprite image;

    [TextArea(3, 8)]
    public string text;
}

public class ComicPrologue : MonoBehaviour
{
    [Header("Comic")]
    [SerializeField] private List<ComicFrame> frames = new();

    [Header("Scene after prologue")]
    [SerializeField] private string gameSceneName = "OfficeScene";

    [Header("Design")]
    [SerializeField]
    private Color dialogueBackground =
        new(0.02f, 0.03f, 0.08f, 0.92f);

    [SerializeField]
    private Color accentColor =
        new(0.16f, 0.55f, 1f, 1f);

    private int currentFrameIndex;

    private Image frameImage;
    private TMP_Text dialogueText;
    private TMP_Text nextButtonText;

    private void Start()
    {
        if (frames.Count == 0)
        {
            Debug.LogError("ComicPrologue: add at least one frame.", this);
            return;
        }

        EnsureEventSystem();
        CreateUi();
        ShowFrame(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Advance();
    }

    private void ShowFrame(int index)
    {
        currentFrameIndex = index;

        ComicFrame frame = frames[currentFrameIndex];

        frameImage.sprite = frame.image;
        dialogueText.text = frame.text;

        bool isLastFrame = currentFrameIndex == frames.Count - 1;

        nextButtonText.text = isLastFrame
            ? "START GAME"
            : "NEXT  →";
    }

    public void Advance()
    {
        if (currentFrameIndex < frames.Count - 1)
        {
            ShowFrame(currentFrameIndex + 1);
            return;
        }

        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError(
                "ComicPrologue: Game Scene Name is empty.",
                this
            );

            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    private void CreateUi()
    {
        GameObject canvasObject = new GameObject(
            "ComicPrologueCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        frameImage = CreateImage(
            "FrameImage",
            canvas.transform,
            Vector2.zero,
            Vector2.one,
            Color.white
        );

        frameImage.preserveAspect = false;

        GameObject dialoguePanel = CreatePanel(
            "DialoguePanel",
            canvas.transform,
            new Vector2(0.04f, 0.04f),
            new Vector2(0.96f, 0.29f),
            dialogueBackground
        );

        Button dialoguePanelButton =
            dialoguePanel.AddComponent<Button>();

        dialoguePanelButton.targetGraphic =
            dialoguePanel.GetComponent<Image>();

        dialoguePanelButton.onClick.AddListener(Advance);

        dialogueText = CreateText(
            "DialogueText",
            dialoguePanel.transform,
            "",
            50,
            TextAlignmentOptions.TopLeft,
            Color.white,
            new Vector2(0.04f, 0.16f),
            new Vector2(0.72f, 0.88f)
        );

        Button nextButton = CreateButton(
            "NextButton",
            dialoguePanel.transform,
            "NEXT  →",
            new Vector2(0.76f, 0.20f),
            new Vector2(0.95f, 0.52f),
            accentColor
        );

        nextButton.onClick.AddListener(Advance);

        nextButtonText = nextButton.GetComponentInChildren<TMP_Text>();
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule)
        );
    }

    private GameObject CreatePanel(
        string objectName,
        Transform parent,
        Vector2 min,
        Vector2 max,
        Color color)
    {
        return CreateImage(
            objectName,
            parent,
            min,
            max,
            color
        ).gameObject;
    }

    private Image CreateImage(
        string objectName,
        Transform parent,
        Vector2 min,
        Vector2 max,
        Color color)
    {
        GameObject obj = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Image)
        );

        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        SetAnchors(rect, min, max);

        Image image = obj.GetComponent<Image>();
        image.color = color;

        return image;
    }

    private Button CreateButton(
        string objectName,
        Transform parent,
        string label,
        Vector2 min,
        Vector2 max,
        Color color)
    {
        Image background = CreateImage(
            objectName,
            parent,
            min,
            max,
            color
        );

        Button button = background.gameObject.AddComponent<Button>();
        button.targetGraphic = background;

        CreateText(
            "Label",
            background.transform,
            label,
            22,
            TextAlignmentOptions.Center,
            Color.white,
            Vector2.zero,
            Vector2.one
        );

        return button;
    }

    private TMP_Text CreateText(
        string objectName,
        Transform parent,
        string value,
        float fontSize,
        TextAlignmentOptions alignment,
        Color color,
        Vector2 min,
        Vector2 max)
    {
        GameObject obj = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        SetAnchors(rect, min, max);

        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.font = TMP_Settings.defaultFontAsset;
        text.text = value;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.fontStyle = FontStyles.Bold;
        text.enableWordWrapping = true;

        return text;
    }

    private void SetAnchors(
        RectTransform rect,
        Vector2 min,
        Vector2 max)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}