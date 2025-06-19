using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    private ItemData itemData;
    public void SetItem(Sprite icon, int quantity)
    {
        iconImage.sprite = icon;
        quantityText.text = quantity.ToString();
        // Tìm lại dữ liệu gốc từ JSON
        itemData = ItemDatabase.Instance.GetItemByIcon(icon); // thêm hàm hỗ trợ này bên dưới
    }
    public void OnClick()
    {
        if (itemData != null)
        {
            ItemDetailPanel.Instance.ShowDetail(itemData);
        }
    }
    
}
