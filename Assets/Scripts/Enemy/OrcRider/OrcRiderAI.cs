using UnityEngine;
using System.Collections;

public class OrcRiderAI : EnemyAI
{
    [Header("Orc Rider Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackDuration = 1.2f;
    [SerializeField] private float attackDelay = 0.4f;

    [Header("Charge Settings")]
    [SerializeField] private float chargeDuration = 1.2f;
    [SerializeField] private float chargeStartDelay = 0.3f;
    [SerializeField] private float chargeSpeed = 4f;
    [SerializeField] private float chargeDamageMultiplier = 1.3f;
    [SerializeField] private float chargeChance = 0.3f;
    [SerializeField] private float chargeCheckInterval = 1.5f;

    private bool isCharging = false;
    private float lastChargeCheckTime = 0f;

    protected override void Start()
    {
        base.Start();

        // Gán lại tầm phát hiện và tầm đánh cho riêng OrcRider
        detectionRange = 7f;
        attackRange = 1.8f;
    }

    protected override void Update()
    {
        base.Update();

        if (!isAttacking && !isCharging && !IsPlayerInAttackRange())
        {
            TryChargeAttack();
        }
    }

    protected override void Attack()
    {
        if (!isAttacking && !isCharging)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void TryChargeAttack()
    {
        if (Time.time - lastChargeCheckTime >= chargeCheckInterval)
        {
            lastChargeCheckTime = Time.time;

            if (IsPlayerInDetectionRange() && Random.value <= chargeChance)
            {
                StartCoroutine(ChargeRoutine());
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(attackDelay);
        rb.linearVelocity = Vector2.zero;

        if (IsPlayerInAttackRange())
        {
            var playerHealth = target?.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackDuration - attackDelay);

        if (animator != null)
            animator.SetBool("IsAttacking", false);

        isAttacking = false;
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(chargeStartDelay);

        if (animator != null)
            animator.SetBool("IsCharging", true);

        Vector2 direction = (target.position - transform.position).normalized;
        float elapsed = 0f;
        bool hasHit = false;

        while (elapsed < (chargeDuration - chargeStartDelay))
        {
            rb.linearVelocity = direction * chargeSpeed;

            if (!hasHit && target != null &&
                Vector2.Distance(transform.position, target.position) <= 1.0f)
            {
                var playerHealth = target.GetComponent<PlayerHealth>();
                playerHealth?.TakeDamage(Mathf.RoundToInt(damage * chargeDamageMultiplier));
                hasHit = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("IsCharging", false);

        isCharging = false;
    }

    protected override void UpdateAnimation()
    {
        if (animator == null) return;

        bool isWalking = !isAttacking && !isCharging &&
                         IsPlayerInDetectionRange() &&
                         !IsPlayerInAttackRange();

        animator.SetBool("IsWalking", isWalking);
    }

    protected override void HandleMovement()
    {
        if (isAttacking || isCharging) return;

        if (IsPlayerInDetectionRange() && !IsPlayerInAttackRange())
        {
            Vector2 direction = (target.position - transform.position).normalized;
            Move(direction);

            if (spriteRenderer != null)
                spriteRenderer.flipX = direction.x < 0;
        }
    }
}
