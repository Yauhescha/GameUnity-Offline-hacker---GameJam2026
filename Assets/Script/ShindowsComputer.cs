using System.Collections.Generic;
using UnityEngine;

public enum PasswordAttemptResult
{
    Success,
    WrongPassword,
    Locked
}

public class ShindowsComputer : Interactable
{
    [Header("Shindows 9")]
    [SerializeField] private string computerName = "DEV-PC-01";

    [Header("Password")]
    [SerializeField] private bool requiresPassword;
    [SerializeField] private string password;
    [SerializeField, Min(1)] private int maxPasswordAttempts = 3;

    [Header("Desktop content")]
    [SerializeField] private List<ShindowsFileData> desktopItems = new();

    [Header("External desktop data")]
    [SerializeField] private TextAsset desktopJson;
    [SerializeField] private bool useJsonIfAssigned = true;

    private int failedAttempts;
    private bool isUnlocked;
    private bool isPermanentlyLocked;

    public string ComputerName => computerName;
    public bool RequiresPassword => requiresPassword;
    public bool IsUnlocked => isUnlocked;
    public bool IsPermanentlyLocked => isPermanentlyLocked;
    public int FailedAttempts => failedAttempts;
    public int RemainingAttempts => maxPasswordAttempts - failedAttempts;
    public IReadOnlyList<ShindowsFileData> DesktopItems => desktopItems;

    public override string InteractionHint => "E — Use computer";

    private void Start()
    {
        if (!useJsonIfAssigned || desktopJson == null)
            return;

        desktopItems = ShindowsJsonLoader.Load(desktopJson);
    }

    public override void Interact()
    {
        if (isPermanentlyLocked)
        {
            EndGameManager.Instance.EndFailure(
                $"{computerName} is permanently locked."
            );
            return;
        }

        Shindows9UI.Instance.Open(this);
    }

    public PasswordAttemptResult TryPassword(string enteredPassword)
    {
        if (!requiresPassword)
        {
            isUnlocked = true;
            return PasswordAttemptResult.Success;
        }

        if (enteredPassword == password)
        {
            isUnlocked = true;
            return PasswordAttemptResult.Success;
        }

        failedAttempts++;

        if (failedAttempts >= maxPasswordAttempts)
        {
            isPermanentlyLocked = true;
            return PasswordAttemptResult.Locked;
        }

        return PasswordAttemptResult.WrongPassword;
    }
}