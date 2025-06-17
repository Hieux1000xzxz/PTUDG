using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private Dictionary<string, int> itemCounts = new Dictionary<string, int>();
    public event Action OnInventoryChanged;


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
        OnInventoryChanged?.Invoke();

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
            {
                itemCounts.Remove(itemName); // XÓA hoàn toàn khỏi inventory
                Debug.Log($"{itemName} đã bị xóa khỏi inventory.");
            }

            OnInventoryChanged?.Invoke();
            Debug.Log($"Removed {itemName}. Remaining: {GetItemCount(itemName)}");
        }
    }


    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(itemCounts);
    }

}
