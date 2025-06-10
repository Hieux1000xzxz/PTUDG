using UnityEngine;
using System.Collections;

public class WolfAI : EnemyAI
{
    [Header("Wolf Settings")]
    [SerializeField] private int damage = 20;
    [SerializeField] private float attackDuration = 1f;
    [SerializeField] private float attackDelay = 0.3f; // Thời gian trước khi gây damage
    protected override void Attack()
    {
        if (!isAttacking) // Chỉ bắt đầu tấn công nếu không đang trong trạng thái tấn công
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        //Khóa vị trí khi tấn công
        // Kích hoạt animation tấn công bằng SetBool
        if (animator != null)
            animator.SetBool("IsAttacking", true);

        // Chờ trước khi gây damage
        yield return new WaitForSeconds(attackDelay);
        rb.linearVelocity = Vector2.zero;

        // Gây damage nếu player trong tầm
        if (IsPlayerInAttackRange())
        {
            var playerHealth = target?.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(damage);
        }

        // Chờ hoàn thành animation
        yield return new WaitForSeconds(attackDuration - attackDelay);

        // Tắt trạng thái tấn công
        if (animator != null)
            animator.SetBool("IsAttacking", false);

        isAttacking = false;
    }

    protected override void UpdateAnimation()
    {
        if (animator == null) return;

        // Mặc định là idle, chỉ walk khi đuổi theo player
        bool isWalking = !isAttacking &&
                        IsPlayerInDetectionRange() &&
                        !IsPlayerInAttackRange();

        animator.SetBool("IsWalking", isWalking);
    }

    protected override void HandleMovement()
    {
        if (isAttacking) return;

        // Di chuyển nếu player trong tầm phát hiện nhưng chưa tới tầm tấn công
        if (IsPlayerInDetectionRange() && !IsPlayerInAttackRange())
        {
            Vector2 direction = (target.position - transform.position).normalized;
            Move(direction);

            // Flip sprite theo hướng di chuyển
            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
    }
}