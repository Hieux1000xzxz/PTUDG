using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider manaSlider;

    private PlayerHealth playerHealth;

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
    }
}
