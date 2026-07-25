using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionObjective : MonoBehaviour
{
    public static MissionObjective Instance { get; private set; }

    [SerializeField]
    private List<string> requiredFileIds = new()
    {
        "architecture",
        "offer",
        "roadmap"
    };

    private void Awake()
    {
        Instance = this;
    }

    public bool TrySendPackage(out string message)
    {
        List<string> missingFiles = requiredFileIds
            .Where(id => !PlayerFlashDrive.Instance.HasFile(id))
            .ToList();

        if (missingFiles.Count > 0)
        {
            message =
                "Transfer cancelled. Missing files:\n• " +
                string.Join("\n• ", missingFiles);

            return false;
        }

        message = "All NEBULA files transferred before lunch ended.";

        EndGameManager.Instance.EndSuccess(message);
        return true;
    }
}