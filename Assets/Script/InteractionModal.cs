using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionModal : MonoBehaviour
{
    public static InteractionModal Instance { get; private set; }
    public bool IsOpen => root.activeSelf;

    [Header("Main")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Buttons")]
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private Button actionButtonPrefab;

    private readonly List<Button> spawnedButtons = new();
    private Interactable currentInteractable;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        root.SetActive(false);
    }

    private void Update()
    {
        if (root.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Open(Interactable interactable)
    {
        currentInteractable = interactable;

        titleText.text = interactable.DisplayName;
        descriptionText.text = interactable.Description;

        ClearButtons();

        foreach (InteractionAction action in interactable.GetActions())
            CreateButton(action);

        root.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Close()
    {
        if (!root.activeSelf)
            return;

        ClearButtons();

        currentInteractable = null;
        root.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CreateButton(InteractionAction action)
    {
        Button button = Instantiate(actionButtonPrefab, buttonsContainer);
        GameAudio.Instance?.RegisterButton(button);

        TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
        buttonText.text = action.ButtonText;

        button.onClick.AddListener(() =>
        {
            if (currentInteractable != null)
                currentInteractable.ExecuteAction(action);
        });

        spawnedButtons.Add(button);
    }

    private void ClearButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }

        spawnedButtons.Clear();
    }

    public void ShowMessage(string title, string message)
    {
        currentInteractable = null;

        titleText.text = title;
        descriptionText.text = message;

        ClearButtons();

        Button closeButton = Instantiate(actionButtonPrefab, buttonsContainer);
        GameAudio.Instance?.RegisterButton(closeButton);

        TMP_Text buttonText = closeButton.GetComponentInChildren<TMP_Text>();
        buttonText.text = "Close";

        closeButton.onClick.AddListener(Close);
        spawnedButtons.Add(closeButton);

        root.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
