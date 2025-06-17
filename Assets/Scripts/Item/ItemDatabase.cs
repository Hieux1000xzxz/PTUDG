using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;
    public List<ItemData> items;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        LoadItemData();
    }

    private void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/items"); // tương ứng Data/items.json
        if (jsonFile != null)
        {
            ItemDataList dataList = JsonUtility.FromJson<ItemDataList>(jsonFile.text);
            items = dataList.items;
            Debug.Log("Loaded " + items.Count + " items.");
        }
        else
        {
            Debug.LogError("Không tìm thấy file JSON!");
        }
    }

    public ItemData GetItemByName(string name)
    {
        return items.Find(i => i.itemName == name);
    }
    public ItemData GetItemByIcon(Sprite sprite)
    {
        return items.Find(i => i.Icon == sprite);
    }

}
