using System.Collections;
using System.Collections.Generic;
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


    private float hitStunDuration = 0.25f;
    private PlayerForm currentForm;

    private Animator animator;
    private PlayerController playerController;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
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
        StartCoroutine(HandleHitStun());
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
                bonusArmor = 7f;
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

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Vô hiệu hóa các chức năng khác nếu cần
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (animator != null)
        {
            StartCoroutine(DisableAnimatorAfterDeath());
        }
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
    private IEnumerator HandleHitStun()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (playerController != null)
        {
            playerController.moveSpeed = 2f;
        }

        yield return new WaitForSeconds(hitStunDuration);

        if (playerController != null)
        {
            playerController.moveSpeed = 5f;
        }
     }
    private IEnumerator DisableAnimatorAfterDeath()
    {
        yield return new WaitForSeconds(1.5f); // Thời gian chờ để animation chết hoàn thành
        animator.enabled = false; // Vô hiệu hóa animator sau khi chết
    }

}
