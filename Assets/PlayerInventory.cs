using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void AddItem(string itemName, int amount = 1)
    {
        if (itemCounts.ContainsKey(itemName))
            itemCounts[itemName] += amount;
        else
            itemCounts[itemName] = amount;

        Debug.Log($"Picked up {itemName}. Total: {itemCounts[itemName]}");
    }

    public int GetItemCount(string itemName)
    {
        return itemCounts.ContainsKey(itemName) ? itemCounts[itemName] : 0;
    }
    public void RemoveItem(string itemName, int amount = 1)
    {
        if (itemCounts.ContainsKey(itemName))
        {
            itemCounts[itemName] -= amount;
            if (itemCounts[itemName] <= 0)
                itemCounts.Remove(itemName);

            Debug.Log($"Removed {itemName}. Remaining: {GetItemCount(itemName)}");
        }
    }
}
