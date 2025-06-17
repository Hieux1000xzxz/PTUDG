using UnityEngine;
[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public string iconPath;

    // Icon được load từ Resources
    public Sprite Icon => Resources.Load<Sprite>(iconPath);
}
