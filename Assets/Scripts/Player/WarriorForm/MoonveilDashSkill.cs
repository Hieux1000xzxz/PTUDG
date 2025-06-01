using System.Collections;
using UnityEngine;

public class MoonveilDashSkill : MonoBehaviour
{
    [Header("Moonveil Dash Settings")]
    public float dashDuration = 0.6f;
    public float hitInterval = 0.15f;
    public int damagePerHit = 10;
    public float attackLength = 1.5f; // Chiều dài của đòn tấn công
    public float attackWidth = 0.4f;
    public Transform attackPoint;
    public float manaCost = 20f;

    [Header("Cooldown Settings")]
    public float cooldownTime = 5f;
    private float cooldownTimer = 0f;

    private bool isDashing = false;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private Animator animator;
    private Vector2 dashStartPosition;
    private PlayerHealth playerHealth;
    private float dashTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }
    public void ActivateMoonveilDash()
    {
        if (isDashing) return;             // Đang thực hiện skill
        if (cooldownTimer > 0f) return;           // Đang cooldown
        if (playerHealth == null) return;
        if (playerHealth.currentMana < manaCost)
        {
            Debug.Log("Không đủ mana để dùng Triple Slash");
            return;
        }

        playerHealth.UseMana(manaCost); // Trừ mana
        cooldownTimer = cooldownTime;

        StartCoroutine(MoonveilDashRoutine());
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private IEnumerator MoonveilDashRoutine()
    {
        isDashing = true;
        dashDirection = playerController.GetLastMoveDirection();
        dashStartPosition = rb.position;
        dashTimer = 0f;
        int hitCount = 0;
        animator.SetBool("isDashing", true);

        while (dashTimer < dashDuration && hitCount < 4)
        {
            // Tấn công mỗi khoảng thời gian
            if (hitCount * hitInterval <= dashTimer)
            {
                Vector2 center = (Vector2)attackPoint.position + dashDirection.normalized * 0.5f;

                Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
                    center,
                    new Vector2(attackLength, attackWidth),
                    0f
                );

                foreach (var enemy in hitEnemies)
                {
                    if (enemy.CompareTag("Enemy"))
                    {
                        enemy.GetComponent<EnemyHealth>()?.TakeDamage(damagePerHit);
                    }
                }
                hitCount++;
            }

            dashTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool("isDashing", false);
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Vector2 pos = (Vector2)attackPoint.position + Vector2.right * 0.5f;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(pos, new Vector3(attackLength, attackWidth, 0));
    }
}
