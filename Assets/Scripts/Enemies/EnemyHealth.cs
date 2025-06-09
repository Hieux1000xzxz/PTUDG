using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private EnemyItemDropper itemDropper;

    //private GameManager gameManager;
    public bool IsDead => currentHealth <= 0; // Thuộc tính kiểm tra trạng thái chết
    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged HealthChanged;

    public GameObject deathPrefab; // Prefab để tạo hiệu ứng chết
    [SerializeField] private GameObject floatingTextPrefab; // Prefab cho text nổi

    private void Awake()
    {
        /*gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }*/
        currentHealth = maxHealth;
        itemDropper = GetComponent<EnemyItemDropper>();

    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} bị trúng đòn! Máu còn lại: {currentHealth}");
        // Gọi sự kiện khi máu thay đổi
        HealthChanged?.Invoke(currentHealth, maxHealth);
        /*if (floatingTextPrefab != null)
        {
            ShowFloatingText(amount);
        }*/
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} đã chết!");
        GameObject x = Instantiate(deathPrefab, transform.position, Quaternion.identity);

        itemDropper?.DropItems(); // Gọi Drop từ script khác

        Destroy(gameObject);
        Destroy(x, 1.5f);
    }
    /*private void ShowFloatingText(int damage)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        FloatingDamageText floating = text.GetComponent<FloatingDamageText>();
        floating.SetDamage(damage);
    }*/
}
