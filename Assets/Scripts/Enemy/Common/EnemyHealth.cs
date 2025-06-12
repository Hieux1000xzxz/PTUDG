using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float hitStunDuration = 0.25f;
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private GameObject deathPrefab;

    private int currentHealth;
    private EnemyHealthBar healthBarInstance;
    private EnemyItemDropper itemDropper;
    private Animator animator;
    private EnemyAI enemyController;
    private bool isDead = false;

    private void Start()
    {
        InitializeComponents();
        SetupHealthBar();
    }

    private void InitializeComponents()
    {
        currentHealth = maxHealth;
        itemDropper = GetComponent<EnemyItemDropper>();
        animator = GetComponent<Animator>();
        enemyController = GetComponent<EnemyAI>();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab == null) return;

        var healthBarPosition = transform.position + new Vector3(-0.4f, 0.8f, 0);
        var healthBarObject = Instantiate(healthBarPrefab, healthBarPosition, Quaternion.identity, transform);
        healthBarInstance = healthBarObject.GetComponent<EnemyHealthBar>();
        healthBarInstance.SetHealth(1f);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || currentHealth <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);
        UpdateHealthBar();

        TriggerHitReaction();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void TriggerHitReaction()
    {
        animator.SetTrigger("Hit");

        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        Invoke(nameof(RestoreEnemyControl), hitStunDuration);
    }

    private void RestoreEnemyControl()
    {
        if (isDead) return;

        if (enemyController != null)
        {
            enemyController.enabled = true;
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarInstance == null) return;

        float healthPercentage = (float)currentHealth / maxHealth;
        healthBarInstance.SetHealth(healthPercentage);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} has died!");
        SpawnDeathEffect();
        DropItems();
        Destroy(gameObject); 
    }

    private void SpawnDeathEffect()
    {
        if (deathPrefab == null) return;

        var deathEffect = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        Destroy(deathEffect, 1.5f);
    }

    private void DropItems()
    {
        itemDropper?.DropItems();
    }
}