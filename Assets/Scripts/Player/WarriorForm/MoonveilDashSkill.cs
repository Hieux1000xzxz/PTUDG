using System.Collections;
using UnityEngine;

public class MoonveilDashSkill : MonoBehaviour
{
    [Header("Moonveil Dash Settings")]
    public float dashDuration = 0.6f;
    public float hitInterval = 0.15f;
    public int damagePerHit = 10;
    public float attackLength = 1.5f;
    public float attackWidth = 0.4f;
    public Transform attackPoint;
    public float manaCost = 20f;
    public string enemyTag = "Enemy"; // Sử dụng tag thay vì layer

    [Header("Cooldown Settings")]
    public float cooldownTime = 5f;
    private float cooldownTimer = 0f;

    private bool isDashing = false;
    private bool canDash = true;
    private Vector2 dashDirection;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private Animator animator;
    private PlayerHealth playerHealth;
    private float dashTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
        else if (!canDash)
        {
            canDash = true;
        }
    }

    public void ActivateMoonveilDash()
    {
        if (!CanActivateDash()) return;

        playerHealth.UseMana(manaCost);
        cooldownTimer = cooldownTime;
        canDash = false;

        StartCoroutine(MoonveilDashRoutine());
    }

    private bool CanActivateDash()
    {
        if (isDashing)
        {
            Debug.Log("Dash is already active");
            return false;
        }

        if (!canDash)
        {
            Debug.Log("Dash is on cooldown");
            return false;
        }

        if (playerHealth == null || playerHealth.currentMana < manaCost)
        {
            Debug.Log("Not enough mana for Moonveil Dash");
            return false;
        }

        return true;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private IEnumerator MoonveilDashRoutine()
    {
        InitializeDash();

        yield return new WaitForSeconds(0.2f); // Startup delay

        int hitCount = 0;
        while (ShouldContinueDash(hitCount))
        {
            if (ShouldHit(hitCount))
            {
                PerformAttack();
                hitCount++;
            }

            dashTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        EndDash();
    }

    private void InitializeDash()
    {
        isDashing = true;
        dashDirection = playerController.GetLastMoveDirection();
        dashTimer = 0f;
        animator.SetBool("isDashing", true);
    }

    private bool ShouldContinueDash(int hitCount)
    {
        return dashTimer < dashDuration && hitCount < 4;
    }

    private bool ShouldHit(int hitCount)
    {
        return hitCount * hitInterval <= dashTimer;
    }

    private void PerformAttack()
    {
        // Tạo một hình chữ nhật tại vị trí tấn công
        Vector2 attackCenter = (Vector2)attackPoint.position + dashDirection.normalized * (attackLength * 0.5f);

        // Lấy tất cả collider trong khu vực
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            attackCenter,
            new Vector2(attackLength, attackWidth),
            Vector2.Angle(Vector2.right, dashDirection)
        );

        foreach (var collider in hitColliders)
        {
            // Kiểm tra theo tag thay vì layer
            if (collider.CompareTag(enemyTag))
            {
                collider.GetComponent<EnemyHealth>()?.TakeDamage(damagePerHit);
            }
        }
    }

    private void EndDash()
    {
        animator.SetBool("isDashing", false);
        isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;

        Vector2 center = (Vector2)attackPoint.position + dashDirection.normalized * (attackLength * 0.5f);
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(
            center,
            Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, dashDirection)),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackLength, attackWidth, 0));
    }
}