using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuntimeUIBootstrap : MonoBehaviour
{
    [Header("Game settings")]
    [SerializeField] private float gameDurationSeconds = 180f;
    [SerializeField] private int flashDriveCapacityMb = 250;

    private Font defaultFont;

    private void Awake()
    {
        EnsureEventSystem();

        if (TMP_Settings.defaultFontAsset == null)
        {
            Debug.LogError(
                "TextMeshPro default font is missing. " +
                "Import TMP Essentials: Window → TextMeshPro → Import TMP Essential Resources."
            );
            return;
        }

        Canvas canvas = CreateCanvas();

        PlayerFlashDrive flashDrive =
            GetOrAddComponent<PlayerFlashDrive>();

        PlayerInventory inventory =
            GetOrAddComponent<PlayerInventory>();

        Shindows9UI shindows =
            GetOrAddComponent<Shindows9UI>();

        EndGameManager endGame =
            GetOrAddComponent<EndGameManager>();

        CountdownTimer timer =
            GetOrAddComponent<CountdownTimer>();

        TMP_Text timerText = BuildHud(canvas.transform);

        BuildShindows(canvas.transform, shindows);

        BuildEndGamePanel(
            canvas.transform,
            endGame
        );

        timer.Initialize(timerText, gameDurationSeconds);
    }

    private void BuildShindows(
        Transform canvasTransform,
        Shindows9UI shindows)
    {
        Color dark = new(0.045f, 0.055f, 0.11f, 0.98f);
        Color panel = new(0.09f, 0.12f, 0.24f, 1f);
        Color blue = new(0.14f, 0.43f, 0.95f, 1f);
        Color cyan = new(0.14f, 0.43f, 0.95f, 1f);
        //Color cyan = new(0.16f, 0.85f, 1f, 1f);
        Color textColor = new(0.90f, 0.96f, 1f, 1f);

        Image rootImage = CreatePanel(
            "Shindows9Root",
            canvasTransform,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Color(0f, 0f, 0f, 0.65f)
        );

        GameObject root = rootImage.gameObject;

        Image windowImage = CreatePanel(
            "ComputerWindow",
            root.transform,
            new Vector2(0.10f, 0.08f),
            new Vector2(0.90f, 0.92f),
            dark
        );

        CanvasGroup windowCanvasGroup =
            windowImage.gameObject.AddComponent<CanvasGroup>();

        Image header = CreatePanel(
            "Header",
            windowImage.transform,
            new Vector2(0f, 0.90f),
            new Vector2(1f, 1f),
            blue
        );

        TMP_Text osTitle = CreateText(
            "OsTitle",
            header.transform,
            "SHINDOWS 9 OS",
            29,
            TextAlignmentOptions.Left,
            textColor,
            new Vector2(0.03f, 0f),
            new Vector2(0.44f, 1f)
        );

        TMP_Text headerComputerName = CreateText(
            "HeaderComputerName",
            header.transform,
            "",
            23,
            TextAlignmentOptions.Left,
            textColor,
            new Vector2(0.45f, 0f),
            new Vector2(0.80f, 1f)
        );

        Button closeComputerButton = CreateButton(
            "CloseComputerButton",
            header.transform,
            "×",
            new Vector2(0.91f, 0.15f),
            new Vector2(0.98f, 0.85f),
            new Color(0.90f, 0.16f, 0.28f),
            28
        );

        Image taskbar = CreatePanel(
            "Taskbar",
            windowImage.transform,
            new Vector2(0f, 0f),
            new Vector2(1f, 0.085f),
            panel
        );

        TMP_Text startText = CreateText(
            "StartText",
            taskbar.transform,
            "START",
            19,
            TextAlignmentOptions.Center,
            cyan,
            new Vector2(0.02f, 0f),
            new Vector2(0.15f, 1f)
        );

        TMP_Text taskbarComputerName = CreateText(
            "TaskbarComputerName",
            taskbar.transform,
            "",
            18,
            TextAlignmentOptions.Left,
            textColor,
            new Vector2(0.17f, 0f),
            new Vector2(0.55f, 1f)
        );

        TMP_Text flashDriveText = CreateText(
            "FlashDriveText",
            taskbar.transform,
            "USB: 0 / 250 MB",
            18,
            TextAlignmentOptions.Right,
            cyan,
            new Vector2(0.60f, 0f),
            new Vector2(0.96f, 1f)
        );

        GameObject passwordPanel = CreatePanel(
            "PasswordPanel",
            windowImage.transform,
            new Vector2(0.22f, 0.28f),
            new Vector2(0.78f, 0.70f),
            panel
        ).gameObject;

        CreateText(
            "PasswordTitle",
            passwordPanel.transform,
            "SECURE LOGIN",
            28,
            TextAlignmentOptions.Center,
            cyan,
            new Vector2(0.08f, 0.70f),
            new Vector2(0.92f, 0.90f)
        );

        TMP_Text passwordMessage = CreateText(
            "PasswordMessage",
            passwordPanel.transform,
            "Password required.",
            19,
            TextAlignmentOptions.Center,
            textColor,
            new Vector2(0.08f, 0.54f),
            new Vector2(0.92f, 0.68f)
        );

        TMP_InputField passwordInput = CreateInputField(
            "PasswordInput",
            passwordPanel.transform,
            new Vector2(0.15f, 0.36f),
            new Vector2(0.85f, 0.50f),
            "Enter password..."
        );

        TMP_Text passwordAttempts = CreateText(
            "PasswordAttempts",
            passwordPanel.transform,
            "",
            16,
            TextAlignmentOptions.Center,
            textColor,
            new Vector2(0.10f, 0.24f),
            new Vector2(0.90f, 0.34f)
        );

        Button unlockButton = CreateButton(
            "UnlockButton",
            passwordPanel.transform,
            "UNLOCK",
            new Vector2(0.33f, 0.08f),
            new Vector2(0.67f, 0.19f),
            blue,
            19
        );

        GameObject desktopPanel = CreatePanel(
            "DesktopPanel",
            windowImage.transform,
            new Vector2(0.02f, 0.10f),
            new Vector2(0.98f, 0.88f),
            new Color(0.06f, 0.08f, 0.17f, 1f)
        ).gameObject;

        TMP_Text currentPathText = CreateText(
            "CurrentPathText",
            desktopPanel.transform,
            "Desktop",
            20,
            TextAlignmentOptions.Left,
            cyan,
            new Vector2(0.03f, 0.89f),
            new Vector2(0.70f, 0.98f)
        );

        Button backButton = CreateButton(
            "BackButton",
            desktopPanel.transform,
            "← Back",
            new Vector2(0.80f, 0.88f),
            new Vector2(0.96f, 0.98f),
            panel,
            15
        );

        Image itemsContainerImage = CreatePanel(
            "ItemsContainer",
            desktopPanel.transform,
            new Vector2(0.03f, 0.05f),
            new Vector2(0.97f, 0.84f),
            new Color(0.03f, 0.04f, 0.10f, 0.45f)
        );

        GridLayoutGroup grid =
            itemsContainerImage.gameObject.AddComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(440f, 150f);
        grid.spacing = new Vector2(18f, 18f);
        grid.padding = new RectOffset(18, 18, 18, 18);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;

        ShindowsItemButton itemTemplate = CreateItemTemplate(
            root.transform,
            panel,
            cyan,
            textColor
        );

        GameObject viewerPanel = CreatePanel(
            "ViewerPanel",
            windowImage.transform,
            new Vector2(0.12f, 0.13f),
            new Vector2(0.88f, 0.86f),
            panel
        ).gameObject;

        TMP_Text viewerTitle = CreateText(
            "ViewerTitle",
            viewerPanel.transform,
            "",
            24,
            TextAlignmentOptions.Left,
            cyan,
            new Vector2(0.05f, 0.87f),
            new Vector2(0.76f, 0.97f)
        );

        Button closeViewerButton = CreateButton(
            "CloseViewerButton",
            viewerPanel.transform,
            "×",
            new Vector2(0.89f, 0.88f),
            new Vector2(0.96f, 0.96f),
            new Color(0.90f, 0.16f, 0.28f),
            24
        );

        GameObject viewerTextRoot = CreatePanel(
            "ViewerTextRoot",
            viewerPanel.transform,
            new Vector2(0.05f, 0.20f),
            new Vector2(0.95f, 0.82f),
            new Color(0.025f, 0.03f, 0.07f, 1f)
        ).gameObject;

        TMP_Text viewerText = CreateText(
            "ViewerText",
            viewerTextRoot.transform,
            "",
            20,
            TextAlignmentOptions.TopLeft,
            textColor,
            new Vector2(0.04f, 0.04f),
            new Vector2(0.96f, 0.96f)
        );

        GameObject viewerImageRoot = CreatePanel(
            "ViewerImageRoot",
            viewerPanel.transform,
            new Vector2(0.05f, 0.20f),
            new Vector2(0.95f, 0.82f),
            new Color(0.025f, 0.03f, 0.07f, 1f)
        ).gameObject;

        Image viewerImage = CreateImage(
            "ViewerImage",
            viewerImageRoot.transform,
            new Vector2(0.05f, 0.05f),
            new Vector2(0.95f, 0.95f),
            Color.white
        );

        viewerImage.preserveAspect = true;

        TMP_Text viewerStatus = CreateText(
            "ViewerStatus",
            viewerPanel.transform,
            "",
            16,
            TextAlignmentOptions.Left,
            textColor,
            new Vector2(0.05f, 0.10f),
            new Vector2(0.58f, 0.17f)
        );

        Button copyToFlashButton = CreateButton(
            "CopyToFlashButton",
            viewerPanel.transform,
            "Copy to USB",
            new Vector2(0.58f, 0.06f),
            new Vector2(0.80f, 0.17f),
            new Color(0.10f, 0.66f, 0.46f),
            16
        );

        GameObject copyingPanel = CreatePanel(
            "CopyingPanel",
            root.transform,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Color(0f, 0f, 0f, 0.82f)
        ).gameObject;

        TMP_Text copyingFileName = CreateText(
            "CopyingFileName",
            copyingPanel.transform,
            "Copying...",
            26,
            TextAlignmentOptions.Center,
            textColor,
            new Vector2(0.20f, 0.57f),
            new Vector2(0.80f, 0.67f)
        );

        Slider copyProgress = CreateSlider(
            "CopyProgress",
            copyingPanel.transform,
            new Vector2(0.25f, 0.43f),
            new Vector2(0.75f, 0.49f),
            cyan
        );

        shindows.Initialize(
            root,
            windowCanvasGroup,
            headerComputerName,
            taskbarComputerName,
            currentPathText,
            flashDriveText,
            closeComputerButton,
            passwordPanel,
            passwordInput,
            passwordMessage,
            passwordAttempts,
            unlockButton,
            desktopPanel,
            itemsContainerImage.transform,
            itemTemplate,
            backButton,
            viewerPanel,
            viewerTitle,
            viewerText,
            viewerImage,
            viewerTextRoot,
            viewerImageRoot,
            viewerStatus,
            closeViewerButton,
            copyToFlashButton,
            copyingPanel,
            copyingFileName,
            copyProgress
        );
    }

    private TMP_Text BuildHud(Transform parent)
    {
        TMP_Text timerText = CreateText(
            "HUD_Timer",
            parent,
            "03:00",
            38,
            TextAlignmentOptions.Center,
            Color.white,
            new Vector2(0.82f, 0.91f),
            new Vector2(0.97f, 0.98f)
        );

        timerText.fontStyle = FontStyles.Bold;
        return timerText;
    }

    private void BuildEndGamePanel(
        Transform parent,
        EndGameManager endGameManager)
    {
        GameObject root = CreatePanel(
            "EndGamePanel",
            parent,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Color(0.025f, 0.02f, 0.06f, 0.94f)
        ).gameObject;

        TMP_Text title = CreateText(
            "Title",
            root.transform,
            "MISSION FAILED",
            48,
            TextAlignmentOptions.Center,
            new Color(1f, 0.30f, 0.30f),
            new Vector2(0.15f, 0.62f),
            new Vector2(0.85f, 0.76f)
        );

        TMP_Text reason = CreateText(
            "Reason",
            root.transform,
            "",
            24,
            TextAlignmentOptions.Center,
            Color.white,
            new Vector2(0.20f, 0.45f),
            new Vector2(0.80f, 0.60f)
        );

        Button restart = CreateButton(
            "RestartButton",
            root.transform,
            "TRY AGAIN",
            new Vector2(0.35f, 0.27f),
            new Vector2(0.65f, 0.36f),
            new Color(0.13f, 0.45f, 0.95f),
            21
        );

        Button quit = CreateButton(
            "QuitButton",
            root.transform,
            "QUIT",
            new Vector2(0.40f, 0.16f),
            new Vector2(0.60f, 0.24f),
            new Color(0.30f, 0.32f, 0.43f),
            17
        );

        endGameManager.Initialize(
            root,
            title,
            reason,
            restart,
            quit
        );
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject(
            "RuntimeUI",
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
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject(
            "EventSystem",
            typeof(EventSystem),
            typeof(StandaloneInputModule)
        );
    }

    private ShindowsItemButton CreateItemTemplate(
        Transform parent,
        Color panelColor,
        Color cyan,
        Color textColor)
    {
        Image background = CreatePanel(
            "ShindowsItemTemplate",
            parent,
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            panelColor
        );

        background.gameObject.SetActive(false);

        Button button = background.gameObject.AddComponent<Button>();
        button.targetGraphic = background;
        GameAudio.Instance?.RegisterButton(button);

        Image icon = CreateImage(
            "Icon",
            background.transform,
            new Vector2(0.08f, 0.44f),
            new Vector2(0.30f, 0.78f),
            cyan
        );

        TMP_Text title = CreateText(
            "Title",
            background.transform,
            "",
            16,
            TextAlignmentOptions.Left,
            textColor,
            new Vector2(0.36f, 0.48f),
            new Vector2(0.94f, 0.78f)
        );

        TMP_Text type = CreateText(
            "Type",
            background.transform,
            "",
            11,
            TextAlignmentOptions.Left,
            new Color(0.60f, 0.72f, 0.92f),
            new Vector2(0.36f, 0.25f),
            new Vector2(0.94f, 0.45f)
        );

        ShindowsItemButton itemButton =
            background.gameObject.AddComponent<ShindowsItemButton>();

        itemButton.Initialize(button, icon, title, type);

        return itemButton;
    }

    private TMP_InputField CreateInputField(
        string objectName,
        Transform parent,
        Vector2 min,
        Vector2 max,
        string placeholderText)
    {
        Image background = CreatePanel(
            objectName,
            parent,
            min,
            max,
            new Color(0.02f, 0.03f, 0.08f, 1f)
        );

        TMP_InputField input =
            background.gameObject.AddComponent<TMP_InputField>();

        input.targetGraphic = background;
        input.contentType = TMP_InputField.ContentType.Password;
        input.lineType = TMP_InputField.LineType.SingleLine;

        TMP_Text placeholder = CreateText(
            "Placeholder",
            background.transform,
            placeholderText,
            18,
            TextAlignmentOptions.Left,
            new Color(0.52f, 0.60f, 0.75f),
            new Vector2(0.05f, 0f),
            new Vector2(0.95f, 1f)
        );

        TMP_Text text = CreateText(
            "Text",
            background.transform,
            "",
            18,
            TextAlignmentOptions.Left,
            Color.white,
            new Vector2(0.05f, 0f),
            new Vector2(0.95f, 1f)
        );

        input.textViewport = background.rectTransform;
        input.textComponent = text;
        input.placeholder = placeholder;

        return input;
    }

    private Slider CreateSlider(
        string objectName,
        Transform parent,
        Vector2 min,
        Vector2 max,
        Color fillColor)
    {
        GameObject sliderObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(Slider)
        );

        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect =
            sliderObject.GetComponent<RectTransform>();

        SetAnchors(sliderRect, min, max);

        Slider slider = sliderObject.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        Image background = CreateImage(
            "Background",
            sliderObject.transform,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Color(0.11f, 0.15f, 0.28f)
        );

        Image fill = CreateImage(
            "Fill",
            sliderObject.transform,
            new Vector2(0f, 0.15f),
            new Vector2(1f, 0.85f),
            fillColor
        );

        slider.fillRect = fill.rectTransform;
        slider.targetGraphic = background;

        return slider;
    }

    private Button CreateButton(
        string objectName,
        Transform parent,
        string text,
        Vector2 min,
        Vector2 max,
        Color color,
        float fontSize)
    {
        Image background = CreatePanel(
            objectName,
            parent,
            min,
            max,
            color
        );

        Button button = background.gameObject.AddComponent<Button>();
        button.targetGraphic = background;
        GameAudio.Instance?.RegisterButton(button);

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.82f, 0.95f, 1f);
        colors.pressedColor = new Color(0.56f, 0.76f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        CreateText(
            "Label",
            background.transform,
            text,
            fontSize,
            TextAlignmentOptions.Center,
            Color.white,
            new Vector2(0f, 0f),
            new Vector2(1f, 1f)
        );

        return button;
    }

    private Image CreatePanel(
        string objectName,
        Transform parent,
        Vector2 min,
        Vector2 max,
        Color color)
    {
        return CreateImage(objectName, parent, min, max, color);
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
        text.text = value;
        text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
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

    private T GetOrAddComponent<T>() where T : Component
    {
        T component = GetComponent<T>();

        if (component == null)
            component = gameObject.AddComponent<T>();

        return component;
    }
}
