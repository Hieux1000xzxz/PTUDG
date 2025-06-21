using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform inventoryGrid;

    private Dictionary<string, Sprite> itemIcons = new();
    private Dictionary<string, GameObject> spawnedItems = new();
    private Dictionary<string, int> lastInventorySnapshot = new();

    public static InventoryUIManager Instance;
    private PlayerInventory lastPlayerInventory;

    private bool needUpdate = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadItemIconsFromJson();

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryChanged += () => needUpdate = true;
            needUpdate = true; // cập nhật lần đầu
        }
    }


    private void Update()
    {
        // Kiểm tra nếu có instance mới của PlayerInventory
        if (PlayerInventory.Instance != null && PlayerInventory.Instance != lastPlayerInventory)
        {
            // Nếu đã có instance cũ thì có thể hủy đăng ký event của nó
            if (lastPlayerInventory != null)
            {
                lastPlayerInventory.OnInventoryChanged -= OnInventoryChangedHandler;
            }
            // Đăng ký sự kiện cho instance mới
            lastPlayerInventory = PlayerInventory.Instance;
            lastPlayerInventory.OnInventoryChanged += OnInventoryChangedHandler;
            needUpdate = true;
        }

        if (needUpdate)
        {
            UpdateInventoryUI();
            needUpdate = false;
        }
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }
    private void OnInventoryChangedHandler()
    {
        needUpdate = true;
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

        // So sánh nhanh snapshot để tránh update thừa
        if (AreInventoriesEqual(lastInventorySnapshot, currentItems))
            return;

        // 1. Xoá UI của item không còn
        foreach (var key in new List<string>(spawnedItems.Keys))
        {
            if (!currentItems.ContainsKey(key))
            {
                Destroy(spawnedItems[key]);
                spawnedItems.Remove(key);
            }
        }

        // 2. Cập nhật/Thêm mới item
        foreach (var item in currentItems)
        {
            string name = item.Key;
            int count = item.Value;

            if (!itemIcons.TryGetValue(name, out var icon))
            {
                Debug.LogWarning($"Không có icon cho item: {name}");
                continue;
            }

            if (spawnedItems.TryGetValue(name, out GameObject obj))
            {
                var ui = obj.GetComponent<InventoryItemUI>();
                ui.SetItem(icon, count);
            }
            else
            {
                GameObject objNew = Instantiate(itemPrefab, inventoryGrid);
                var ui = objNew.GetComponent<InventoryItemUI>();
                ui.SetItem(icon, count);
                spawnedItems.Add(name, objNew);
            }
        }

        // Cập nhật snapshot
        lastInventorySnapshot = new Dictionary<string, int>(currentItems);
    }

    private bool AreInventoriesEqual(Dictionary<string, int> a, Dictionary<string, int> b)
    {
        if (a.Count != b.Count) return false;

        foreach (var pair in a)
        {
            if (!b.TryGetValue(pair.Key, out int value) || value != pair.Value)
                return false;
        }

        return true;
    }
}
