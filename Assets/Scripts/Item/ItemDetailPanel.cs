using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    public static ItemDetailPanel Instance;

    public Image iconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // Ẩn lúc đầu
    }

    public void ShowDetail(ItemData data)
    {
        iconImage.sprite = data.Icon;
        itemNameText.text = data.itemName;
        descriptionText.text = data.description;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
