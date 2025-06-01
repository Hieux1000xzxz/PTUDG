using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Mana")]
    public float maxMana = 50f;
    public float currentMana;

    [Header("Defense")]
    public float armor = 10f;

    [Header("Regen")]
    public float healthRegenRate = 0f;
    public float manaRegenRate = 5f;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }

    private void Update()
    {
        Regen();
    }

    private void Regen()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + healthRegenRate * Time.deltaTime, maxHealth);
        }

        if (currentMana < maxMana)
        {
            currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
        }
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor, 1f); // Không bao giờ nhỏ hơn 1 sát thương
        currentHealth -= effectiveDamage;

        Debug.Log($"Người chơi nhận {effectiveDamage} sát thương. Máu còn lại: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Hồi {amount} máu. Máu hiện tại: {currentHealth}");
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }

    public void UseMana(float amount)
    {
        if (HasEnoughMana(amount))
        {
            currentMana -= amount;
            Debug.Log($"Đã sử dụng {amount} mana. Mana còn lại: {currentMana}");
        }
        else
        {
            Debug.Log("Không đủ mana!");
        }
    }

    public void RecoverMana(float amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        Debug.Log($"Hồi {amount} mana. Mana hiện tại: {currentMana}");
    }

    public void ApplyStatsForForm(PlayerForm form)
    {
        switch (form)
        {
            case PlayerForm.Warrior:
                maxHealth = 100f;
                maxMana = 30f;
                armor = 20f;
                break;

            case PlayerForm.Mage:
                maxHealth = 80f;
                maxMana = 80f;
                armor = 5f;
                break;
        }

        // Không khôi phục — chỉ giới hạn nếu vượt quá max mới
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);

        Debug.Log($"Đã cập nhật chỉ số cho dạng {form}: Máu {currentHealth}/{maxHealth}, Mana {currentMana}/{maxMana}, Giáp {armor}");
    }

    private void Die()
    {
        Debug.Log("Người chơi đã chết.");
        // Gọi animation chết, disable điều khiển, hoặc load lại scene
    }
}
