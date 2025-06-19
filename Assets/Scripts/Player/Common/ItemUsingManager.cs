using UnityEngine;

public class ItemUsingManager : MonoBehaviour
{
    public PlayerHealth player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryUseHealthPotion();

        if (Input.GetKeyDown(KeyCode.R))
            TryUseManaPotion();
    }

    private void TryUseHealthPotion()
    {
        var inv = PlayerInventory.Instance;

        if (inv.GetItemCount("Health Potion Large") > 0)
        {
            player.UseHealthPotion(50); // lượng hồi tùy chỉnh
            inv.RemoveItem("Health Potion Large");

        }
        else if (inv.GetItemCount("Health Potion Small") > 0)
        {
            player.UseHealthPotion(25);
            inv.RemoveItem("Health Potion Small");

        }
        else
        {
            Debug.Log("Không có bình máu nào để sử dụng.");
        }
    }

    private void TryUseManaPotion()
    {
        var inv = PlayerInventory.Instance;

        if (inv.GetItemCount("Mana Potion Large") > 0)
        {
            inv.RemoveItem("Mana Potion Large");
            player.UseManaPotion(50);
        }
        else if (inv.GetItemCount("Mana Potion Small") > 0)
        {
            inv.RemoveItem("Mana Potion Small");
            player.UseManaPotion(25);
        }
        else
        {
            Debug.Log("Không có bình mana nào để sử dụng.");
        }
    }
}
