using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject itemPrefab; // Prefab cho item UI
    public Transform inventoryGrid; // Grid chứa các item UI

    private Dictionary<string, Sprite> itemIcons = new();
    private Dictionary<string, GameObject> spawnedItems = new();

    private void Start()
    {
        LoadItemIconsFromJson();

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += UpdateInventoryUI;
            UpdateInventoryUI(); // Cập nhật ban đầu
        }
    }

    private void LoadItemIconsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/items");
        if (jsonFile != null)
        {
            ItemDataList dataList = JsonUtility.FromJson<ItemDataList>(jsonFile.text);
            foreach (var item in dataList.items)
            {
                if (!itemIcons.ContainsKey(item.itemName))
                {
                    itemIcons[item.itemName] = item.Icon;
                }
            }
            Debug.Log($"Đã load {itemIcons.Count} item từ JSON.");
        }
        else
        {
            Debug.LogError("Không tìm thấy file Data/items.json trong Resources!");
        }
    }

    private void UpdateInventoryUI()
    {
        var currentItems = PlayerInventory.Instance.GetAllItems();

        // 1. Xoá UI của item không còn trong inventory
        List<string> keysToRemove = new List<string>();
        foreach (var kvp in spawnedItems)
        {
            if (!currentItems.ContainsKey(kvp.Key))
            {
                Destroy(kvp.Value); // Hủy object trên UI
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            spawnedItems.Remove(key);
        }

        // 2. Cập nhật hoặc tạo mới các item còn lại
        foreach (var item in currentItems)
        {
            string name = item.Key;
            int count = item.Value;

            if (!itemIcons.ContainsKey(name))
            {
                Debug.LogWarning($"Không có icon cho item: {name}");
                continue;
            }

            if (spawnedItems.ContainsKey(name))
            {
                var ui = spawnedItems[name].GetComponent<InventoryItemUI>();
                ui.SetItem(itemIcons[name], count);
            }
            else
            {
                GameObject obj = Instantiate(itemPrefab, inventoryGrid);
                var ui = obj.GetComponent<InventoryItemUI>();
                ui.SetItem(itemIcons[name], count);
                spawnedItems.Add(name, obj);
            }
        }
    }
}