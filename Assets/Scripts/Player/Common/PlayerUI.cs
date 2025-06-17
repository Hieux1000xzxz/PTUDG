using NUnit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;

    private PlayerHealth playerHealth;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI healthPotionText;
    public TextMeshProUGUI manaPotionText;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.maxHealth;
            manaSlider.maxValue = playerHealth.maxMana;

            healthSlider.value = playerHealth.currentHealth;
            manaSlider.value = playerHealth.currentMana;
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        // Cập nhật giá trị theo thời gian
        healthSlider.maxValue = playerHealth.maxHealth;
        manaSlider.maxValue = playerHealth.maxMana;

        healthSlider.value = playerHealth.currentHealth;
        manaSlider.value = playerHealth.currentMana;

        hpText.text = $"HP: {Mathf.RoundToInt(playerHealth.currentHealth)}/{Mathf.RoundToInt(playerHealth.maxHealth)}";
        manaText.text = $"Mana: {Mathf.RoundToInt(playerHealth.currentMana)}/{Mathf.RoundToInt(playerHealth.maxMana)}";
        armorText.text = $"Armor: {playerHealth.armor}";
        var inv = PlayerInventory.Instance;
        healthPotionText.text = $"x{inv.GetItemCount("Health Potion Large") + inv.GetItemCount("Health Potion Small")}";
        manaPotionText.text = $"x{inv.GetItemCount("Mana Potion Large") + inv.GetItemCount("Mana Potion Small")}";
    }
}
