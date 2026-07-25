using System.Collections.Generic;
using UnityEngine;

public enum FlashDriveCopyResult
{
    Success,
    AlreadyCopied,
    NotEnoughSpace
}

public class PlayerFlashDrive : MonoBehaviour
{
    public static PlayerFlashDrive Instance { get; private set; }

    [SerializeField, Min(1)] private int capacityMb = 250;

    private readonly HashSet<string> copiedFileIds = new();
    private int usedSpaceMb;

    public int CapacityMb => capacityMb;
    public int UsedSpaceMb => usedSpaceMb;
    public int FreeSpaceMb => capacityMb - usedSpaceMb;

    private void Awake()
    {
        Instance = this;
    }

    public FlashDriveCopyResult CanCopy(ShindowsFileData file)
    {
        if (copiedFileIds.Contains(file.Id))
            return FlashDriveCopyResult.AlreadyCopied;

        if (file.SizeMb > FreeSpaceMb)
            return FlashDriveCopyResult.NotEnoughSpace;

        return FlashDriveCopyResult.Success;
    }

    public FlashDriveCopyResult Copy(ShindowsFileData file)
    {
        FlashDriveCopyResult result = CanCopy(file);

        if (result != FlashDriveCopyResult.Success)
            return result;

        copiedFileIds.Add(file.Id);
        usedSpaceMb += file.SizeMb;

        Debug.Log($"Copied to flash drive: {file.DisplayName}");
        return FlashDriveCopyResult.Success;
    }

    public bool HasFile(string fileId)
    {
        return copiedFileIds.Contains(fileId);
    }
}