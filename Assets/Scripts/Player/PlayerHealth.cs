using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Base Stats (used as upgrade source)")]
    public float baseHealth = 100f;
    public float baseMana = 50f;
    public float baseArmor = 10f;

    [Header("Current Stats")]
    public float maxHealth;
    public float currentHealth;

    public float maxMana;
    public float currentMana;

    public float armor;

    [Header("Regeneration")]
    public float baseHealthRegen = 0f;
    public float baseManaRegen = 2f;

    public float healthRegenRate;
    public float manaRegenRate;

    private PlayerForm currentForm;

    private void Awake()
    {
        // Không gọi SetForm ở đây, để PlayerController quyết định
        // Khởi tạo currentHealth và currentMana hợp lý, giả sử bằng base stats ban đầu
        maxHealth = baseHealth;
        currentHealth = maxHealth;

        maxMana = baseMana;
        currentMana = maxMana;

        armor = baseArmor;

        healthRegenRate = baseHealthRegen;
        manaRegenRate = baseManaRegen;
    }

    private void Update()
    {
        Regen();
    }

    private void Regen()
    {
        if (currentHealth < maxHealth)
            currentHealth = Mathf.Min(currentHealth + healthRegenRate * Time.deltaTime, maxHealth);

        if (currentMana < maxMana)
            currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
    }

    public void TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(damage - armor, 1f);
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

    public bool HasEnoughMana(float amount) => currentMana >= amount;

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

    public void SetForm(PlayerForm form)
    {
        currentForm = form;

        // Mỗi form có bonus riêng
        float bonusHealth = 0;
        float bonusMana = 0;
        float bonusArmor = 0;
        float bonusHealthRegen = 0;
        float bonusManaRegen = 0;

        switch (form)
        {
            case PlayerForm.Warrior:
                bonusHealth = 20f;
                bonusArmor = 5f;
                bonusManaRegen = -1f; // giảm tốc độ hồi mana
                break;

            case PlayerForm.Mage:
                bonusMana = 50f;
                bonusArmor = -5f;
                bonusManaRegen = 4f; // hồi mana nhanh hơn
                break;
        }

        // Tính toán các chỉ số cuối cùng
        maxHealth = baseHealth + bonusHealth;
        maxMana = baseMana + bonusMana;
        armor = baseArmor + bonusArmor;

        healthRegenRate = Mathf.Max(0, baseHealthRegen + bonusHealthRegen);
        manaRegenRate = Mathf.Max(0, baseManaRegen + bonusManaRegen);

        // Giữ lại giá trị hiện tại không vượt quá max mới
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);

        Debug.Log($"Chuyển sang dạng {form}: Máu {currentHealth}/{maxHealth}, Mana {currentMana}/{maxMana}, Giáp {armor}");
    }

    private void Die()
    {
        Debug.Log("Người chơi đã chết.");
        // Trigger animation, respawn, scene reload, v.v.
    }

    // Các phương thức nâng cấp dùng base stat
    public void UpgradeHealth(float amount)
    {
        baseHealth += amount;
        SetForm(currentForm);
    }

    public void UpgradeMana(float amount)
    {
        baseMana += amount;
        SetForm(currentForm);
    }

    public void UpgradeArmor(float amount)
    {
        baseArmor += amount;
        SetForm(currentForm);
    }

    public void UpgradeManaRegen(float amount)
    {
        baseManaRegen += amount;
        SetForm(currentForm);
    }

    public void UpgradeHealthRegen(float amount)
    {
        baseHealthRegen += amount;
        SetForm(currentForm);
    }
}
