using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private readonly HashSet<string> itemIds = new();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return;

        itemIds.Add(itemId);
        Debug.Log($"Item acquired: {itemId}");
    }

    public bool HasItem(string itemId)
    {
        return itemIds.Contains(itemId);
    }
}