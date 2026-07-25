using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShindowsItemButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text typeText;

    [Header("Colors")]
    [SerializeField] private Color folderColor = new(0.20f, 0.72f, 1f);
    [SerializeField] private Color textColor = new(1f, 0.72f, 0.25f);
    [SerializeField] private Color imageColor = new(0.82f, 0.35f, 1f);

    public void Bind(ShindowsFileData data, Action onClick)
    {
        titleText.text = data.DisplayName;

        switch (data.EntryType)
        {
            case ShindowsEntryType.Folder:
                icon.color = folderColor;
                typeText.text = "FOLDER";
                break;

            case ShindowsEntryType.TextFile:
                icon.color = textColor;
                typeText.text = $"{data.SizeMb} MB • TEXT";
                break;

            case ShindowsEntryType.ImageFile:
                icon.color = imageColor;
                typeText.text = $"{data.SizeMb} MB • IMAGE";
                break;

            case ShindowsEntryType.Executable:
                icon.color = new Color(0.35f, 1f, 0.60f);
                typeText.text = "SYSTEM • EXEC";
                break;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }

    public void Initialize(
    Button newButton,
    Image newIcon,
    TMP_Text newTitleText,
    TMP_Text newTypeText)
    {
        button = newButton;
        icon = newIcon;
        titleText = newTitleText;
        typeText = newTypeText;
    }
}
