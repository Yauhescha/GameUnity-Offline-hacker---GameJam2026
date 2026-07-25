using System;
using UnityEngine;

public enum ShindowsEntryType
{
    Folder,
    TextFile,
    ImageFile,
    Executable
}

[Serializable]
public class ShindowsFileData
{
    public string Id;
    public string DisplayName;
    public string ParentPath;
    public ShindowsEntryType EntryType;
    public int SizeMb;
    public bool IsCopyable;
    public string TextContent;
    public Sprite ImageContent;
}