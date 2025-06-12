using UnityEngine;

public class SlimeAI : EnemyAI
{
    [Header("Slime Settings")]
    public int damage = 10;
    public Transform attackPoint;
    private Vector2 roamTarget;
    private bool hasTarget = false;
    private float roamTimer = 0f;
    [SerializeField] private float roamInterval = 2f; // Thời gian đổi hướng

    protected override void HandleMovement()
    {
        roamTimer -= Time.deltaTime;
        // Đổi hướng nếu hết thời gian hoặc đã đến gần đích
        if (!hasTarget || Vector2.Distance(rb.position, roamTarget) < 0.1f || roamTimer <= 0f)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            roamTarget = rb.position + randomDir * 3f;
            hasTarget = true;
            roamTimer = roamInterval;
        }
        // Di chuyển về phía roamTarget
        Vector2 direction = (roamTarget - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        // Nếu đã đến gần điểm đích, sẽ chọn hướng mới ở frame sau
        if (Vector2.Distance(rb.position, roamTarget) < 0.1f)
        {
            hasTarget = false;
        }
        if (spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0;
    }
    protected override void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (hit.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(damage);
                }
                break;
            }
        }
    }

    protected override void UpdateAnimation() { }
 

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}