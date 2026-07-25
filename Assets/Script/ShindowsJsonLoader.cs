using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShindowsComputerJson
{
    public List<ShindowsEntryJson> items;
}

[Serializable]
public class ShindowsEntryJson
{
    public string id;
    public string displayName;
    public string parentPath;
    public string entryType;
    public int sizeMb;
    public bool isCopyable;
    public string textContent;
    public string imageResourcePath;
}

public static class ShindowsJsonLoader
{
    public static List<ShindowsFileData> Load(TextAsset jsonFile)
    {
        ShindowsComputerJson config =
            JsonUtility.FromJson<ShindowsComputerJson>(jsonFile.text);

        List<ShindowsFileData> result = new();

        if (config == null || config.items == null)
            return result;

        foreach (ShindowsEntryJson item in config.items)
        {
            if (!Enum.TryParse(
                    item.entryType,
                    true,
                    out ShindowsEntryType entryType))
            {
                entryType = ShindowsEntryType.TextFile;
            }

            Sprite sprite = null;

            if (!string.IsNullOrWhiteSpace(item.imageResourcePath))
                sprite = Resources.Load<Sprite>(item.imageResourcePath);

            result.Add(new ShindowsFileData
            {
                Id = item.id,
                DisplayName = item.displayName,
                ParentPath = item.parentPath,
                EntryType = entryType,
                SizeMb = Mathf.Max(1, item.sizeMb),
                IsCopyable = item.isCopyable,
                TextContent = item.textContent,
                ImageContent = sprite
            });
        }

        return result;
    }
}