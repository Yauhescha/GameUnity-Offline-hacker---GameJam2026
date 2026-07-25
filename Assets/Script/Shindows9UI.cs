using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shindows9UI : MonoBehaviour
{
    public static Shindows9UI Instance { get; private set; }

    [Header("Root")]
    [SerializeField] private GameObject root;
    [SerializeField] private CanvasGroup computerWindowCanvasGroup;

    [Header("Header and taskbar")]
    [SerializeField] private TMP_Text headerComputerNameText;
    [SerializeField] private TMP_Text taskbarComputerNameText;
    [SerializeField] private TMP_Text currentPathText;
    [SerializeField] private TMP_Text flashDriveText;
    [SerializeField] private Button closeComputerButton;

    [Header("Password screen")]
    [SerializeField] private GameObject passwordPanel;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text passwordMessageText;
    [SerializeField] private TMP_Text passwordAttemptsText;
    [SerializeField] private Button unlockButton;

    [Header("Desktop / Explorer")]
    [SerializeField] private GameObject desktopPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private ShindowsItemButton itemButtonPrefab;
    [SerializeField] private Button backButton;

    [Header("File viewer")]
    [SerializeField] private GameObject viewerPanel;
    [SerializeField] private TMP_Text viewerTitleText;
    [SerializeField] private TMP_Text viewerTextContent;
    [SerializeField] private Image viewerImage;
    [SerializeField] private GameObject viewerTextRoot;
    [SerializeField] private GameObject viewerImageRoot;
    [SerializeField] private TMP_Text viewerStatusText;
    [SerializeField] private Button closeViewerButton;
    [SerializeField] private Button copyToFlashButton;

    [Header("Copying overlay")]
    [SerializeField] private GameObject copyingPanel;
    [SerializeField] private TMP_Text copyingFileNameText;
    [SerializeField] private Slider copyProgressSlider;

    [Header("Copy settings")]
    [SerializeField, Min(0.1f)] private float megabytesPerSecond = 15f;
    [SerializeField, Min(0.1f)] private float minimumCopyDuration = 0.8f;

    private readonly List<ShindowsItemButton> spawnedItems = new();

    private ShindowsComputer currentComputer;
    private ShindowsFileData selectedFile;
    private string currentPath = "";
    private bool isCopying;
    private bool suppressPasswordInputSound;
    private int previousPasswordLength;

    public bool IsOpen => root.activeSelf;

    public void Initialize(
    GameObject newRoot,
    CanvasGroup newComputerWindowCanvasGroup,
    TMP_Text newHeaderComputerNameText,
    TMP_Text newTaskbarComputerNameText,
    TMP_Text newCurrentPathText,
    TMP_Text newFlashDriveText,
    Button newCloseComputerButton,
    GameObject newPasswordPanel,
    TMP_InputField newPasswordInput,
    TMP_Text newPasswordMessageText,
    TMP_Text newPasswordAttemptsText,
    Button newUnlockButton,
    GameObject newDesktopPanel,
    Transform newItemsContainer,
    ShindowsItemButton newItemButtonPrefab,
    Button newBackButton,
    GameObject newViewerPanel,
    TMP_Text newViewerTitleText,
    TMP_Text newViewerTextContent,
    Image newViewerImage,
    GameObject newViewerTextRoot,
    GameObject newViewerImageRoot,
    TMP_Text newViewerStatusText,
    Button newCloseViewerButton,
    Button newCopyToFlashButton,
    GameObject newCopyingPanel,
    TMP_Text newCopyingFileNameText,
    Slider newCopyProgressSlider)
    {
        root = newRoot;
        computerWindowCanvasGroup = newComputerWindowCanvasGroup;

        headerComputerNameText = newHeaderComputerNameText;
        taskbarComputerNameText = newTaskbarComputerNameText;
        currentPathText = newCurrentPathText;
        flashDriveText = newFlashDriveText;
        closeComputerButton = newCloseComputerButton;

        passwordPanel = newPasswordPanel;
        passwordInput = newPasswordInput;
        passwordMessageText = newPasswordMessageText;
        passwordAttemptsText = newPasswordAttemptsText;
        unlockButton = newUnlockButton;

        desktopPanel = newDesktopPanel;
        itemsContainer = newItemsContainer;
        itemButtonPrefab = newItemButtonPrefab;
        backButton = newBackButton;

        viewerPanel = newViewerPanel;
        viewerTitleText = newViewerTitleText;
        viewerTextContent = newViewerTextContent;
        viewerImage = newViewerImage;
        viewerTextRoot = newViewerTextRoot;
        viewerImageRoot = newViewerImageRoot;
        viewerStatusText = newViewerStatusText;
        closeViewerButton = newCloseViewerButton;
        copyToFlashButton = newCopyToFlashButton;

        copyingPanel = newCopyingPanel;
        copyingFileNameText = newCopyingFileNameText;
        copyProgressSlider = newCopyProgressSlider;

        root.SetActive(false);
        passwordPanel.SetActive(false);
        desktopPanel.SetActive(false);
        viewerPanel.SetActive(false);
        copyingPanel.SetActive(false);

        closeComputerButton.onClick.AddListener(Close);
        unlockButton.onClick.AddListener(SubmitPassword);
        backButton.onClick.AddListener(GoBack);
        closeViewerButton.onClick.AddListener(CloseViewer);
        copyToFlashButton.onClick.AddListener(CopySelectedFile);

        passwordInput.onSubmit.AddListener(_ => SubmitPassword());
        passwordInput.onValueChanged.AddListener(OnPasswordInputChanged);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsOpen || isCopying)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (viewerPanel.activeSelf)
                CloseViewer();
            else
                Close();
        }
    }

    public void Open(ShindowsComputer computer)
    {
        if (EndGameManager.Instance.IsFinished)
            return;

        currentComputer = computer;
        selectedFile = null;
        currentPath = "";
        isCopying = false;

        root.SetActive(true);

        headerComputerNameText.text = computer.ComputerName;
        taskbarComputerNameText.text = computer.ComputerName;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (computer.RequiresPassword && !computer.IsUnlocked)
            ShowPasswordScreen();
        else
            ShowDesktop();
    }

    public void Close()
    {
        if (isCopying)
            return;

        ClearItems();

        currentComputer = null;
        selectedFile = null;

        root.SetActive(false);
        viewerPanel.SetActive(false);
        passwordPanel.SetActive(false);
        desktopPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ForceClose()
    {
        isCopying = false;
        root.SetActive(false);
        viewerPanel.SetActive(false);
        passwordPanel.SetActive(false);
        desktopPanel.SetActive(false);
        copyingPanel.SetActive(false);
    }

    private void ShowPasswordScreen()
    {
        passwordPanel.SetActive(true);
        desktopPanel.SetActive(false);
        viewerPanel.SetActive(false);

        SetPasswordInputText("");
        passwordInput.ActivateInputField();

        passwordMessageText.text = "Password required.";
        passwordMessageText.color = Color.white;

        UpdateAttemptsText();
    }

    private void SubmitPassword()
    {
        if (currentComputer == null || isCopying)
            return;

        PasswordAttemptResult result =
            currentComputer.TryPassword(passwordInput.text);

        SetPasswordInputText("");

        switch (result)
        {
            case PasswordAttemptResult.Success:
                ShowDesktop();
                break;

            case PasswordAttemptResult.WrongPassword:
                GameAudio.Instance?.PlayPasswordError();
                passwordMessageText.text = "Incorrect password.";
                passwordMessageText.color = new Color(1f, 0.35f, 0.35f);
                UpdateAttemptsText();
                passwordInput.ActivateInputField();
                break;

            case PasswordAttemptResult.Locked:
                ForceClose();

                EndGameManager.Instance.EndFailure(
                    $"{currentComputer.ComputerName} locked after three failed password attempts."
                );
                break;
        }
    }

    private void UpdateAttemptsText()
    {
        passwordAttemptsText.text =
            $"Attempts remaining: {currentComputer.RemainingAttempts}";
    }

    private void ShowDesktop()
    {
        passwordPanel.SetActive(false);
        desktopPanel.SetActive(true);
        viewerPanel.SetActive(false);

        RefreshUsbText();
        RenderCurrentDirectory();
    }

    private void RenderCurrentDirectory()
    {
        ClearItems();

        currentPathText.text = string.IsNullOrEmpty(currentPath)
            ? "Desktop"
            : $"Desktop / {currentPath}";

        backButton.interactable = !string.IsNullOrEmpty(currentPath);

        // Сначала папки, затем файлы.
        RenderEntries(ShindowsEntryType.Folder);
        RenderEntries(ShindowsEntryType.TextFile);
        RenderEntries(ShindowsEntryType.ImageFile);
        RenderEntries(ShindowsEntryType.Executable);
    }


    private void RenderEntries(ShindowsEntryType type)
    {
        foreach (ShindowsFileData item in currentComputer.DesktopItems)
        {
            if (item.EntryType != type)
                continue;

            if (NormalizePath(item.ParentPath) != currentPath)
                continue;

            ShindowsItemButton itemButton =
                Instantiate(itemButtonPrefab, itemsContainer);
            itemButton.gameObject.SetActive(true);

            itemButton.Bind(item, () => OpenEntry(item));
            spawnedItems.Add(itemButton);
        }
    }

    private void OpenEntry(ShindowsFileData item)
    {
        if (isCopying)
            return;

        if (item.EntryType == ShindowsEntryType.Folder)
        {
            currentPath = string.IsNullOrEmpty(currentPath)
                ? item.DisplayName
                : $"{currentPath}/{item.DisplayName}";

            RenderCurrentDirectory();
            return;
        }

        OpenFileViewer(item);
    }

    private void GoBack()
    {
        if (string.IsNullOrEmpty(currentPath))
            return;

        int lastSeparatorIndex = currentPath.LastIndexOf('/');

        currentPath = lastSeparatorIndex < 0
            ? ""
            : currentPath.Substring(0, lastSeparatorIndex);

        RenderCurrentDirectory();
    }

    private void OpenFileViewer(ShindowsFileData file)
    {
        selectedFile = file;
        viewerPanel.SetActive(true);

        viewerTitleText.text = file.DisplayName;
        viewerStatusText.text = "";

        bool isText = file.EntryType != ShindowsEntryType.ImageFile;

        viewerTextRoot.SetActive(isText);
        viewerImageRoot.SetActive(!isText);

        if (isText)
            viewerTextContent.text = file.TextContent;
        else
            viewerImage.sprite = file.ImageContent;

        bool isExecutable = file.EntryType == ShindowsEntryType.Executable;

        copyToFlashButton.gameObject.SetActive(
            file.IsCopyable || isExecutable
        );

        if (file.IsCopyable)
        {
            copyToFlashButton.GetComponentInChildren<TMP_Text>().text =
                $"Copy to USB ({file.SizeMb} MB)";
        }
        else if (isExecutable)
        {
            copyToFlashButton.GetComponentInChildren<TMP_Text>().text =
                "SEND PACKAGE";
        }
    }

    private void CloseViewer()
    {
        if (isCopying)
            return;

        viewerPanel.SetActive(false);
        selectedFile = null;
    }

    private void CopySelectedFile()
    {

        if (selectedFile == null || isCopying)
            return;

        if (selectedFile.EntryType == ShindowsEntryType.Executable)
        {
            RunExecutable();
            return;
        }

        FlashDriveCopyResult preCheck =
            PlayerFlashDrive.Instance.CanCopy(selectedFile);

        if (preCheck == FlashDriveCopyResult.AlreadyCopied)
        {
            ShowViewerError("This file is already on the USB drive.");
            return;
        }

        if (preCheck == FlashDriveCopyResult.NotEnoughSpace)
        {
            ShowViewerError("Not enough free space on the USB drive.");
            return;
        }

        StartCoroutine(CopyFileRoutine(selectedFile));
    }

    private void RunExecutable()
    {
        if (selectedFile.Id != "send_package")
        {
            ShowViewerError("This executable cannot be launched.");
            return;
        }

        if (MissionObjective.Instance == null)
        {
            ShowViewerError(
                "Mission system unavailable. Add MissionObjective to GameManager."
            );
            return;
        }

        bool success = MissionObjective.Instance.TrySendPackage(
            out string message
        );

        if (!success)
            ShowViewerError(message);
    }

    private IEnumerator CopyFileRoutine(ShindowsFileData file)
    {
        isCopying = true;

        computerWindowCanvasGroup.interactable = false;
        computerWindowCanvasGroup.blocksRaycasts = false;

        copyingPanel.SetActive(true);
        copyingFileNameText.text = $"Copying {file.DisplayName}...";
        copyProgressSlider.value = 0f;

        float duration = Mathf.Max(
            minimumCopyDuration,
            file.SizeMb / megabytesPerSecond
        );

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            copyProgressSlider.value = elapsed / duration;
            yield return null;
        }

        PlayerFlashDrive.Instance.Copy(file);

        copyingPanel.SetActive(false);

        computerWindowCanvasGroup.interactable = true;
        computerWindowCanvasGroup.blocksRaycasts = true;

        isCopying = false;

        RefreshUsbText();

        if (viewerPanel.activeSelf)
        {
            viewerStatusText.color = new Color(0.35f, 1f, 0.60f);
            viewerStatusText.text = "Copy complete.";
        }
    }

    private void RefreshUsbText()
    {
        PlayerFlashDrive drive = PlayerFlashDrive.Instance;

        flashDriveText.text =
            $"USB: {drive.UsedSpaceMb} / {drive.CapacityMb} MB";
    }

    private void ShowViewerError(string message)
    {
        viewerStatusText.color = new Color(1f, 0.35f, 0.35f);
        viewerStatusText.text = message;
    }

    private void OnPasswordInputChanged(string value)
    {
        if (!suppressPasswordInputSound &&
            value.Length != previousPasswordLength)
        {
            GameAudio.Instance?.PlayPasswordKey();
        }

        previousPasswordLength = value.Length;
    }

    private void SetPasswordInputText(string value)
    {
        suppressPasswordInputSound = true;
        passwordInput.text = value;
        previousPasswordLength = value.Length;
        suppressPasswordInputSound = false;
    }

    private void ClearItems()
    {
        foreach (ShindowsItemButton item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawnedItems.Clear();
    }

    private static string NormalizePath(string path)
    {
        return string.IsNullOrWhiteSpace(path)
            ? ""
            : path.Trim().Trim('/');
    }
}
