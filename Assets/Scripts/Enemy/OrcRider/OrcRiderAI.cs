using UnityEngine;
using System.Collections;

public class OrcRiderAI : EnemyAI
{
    [Header("Orc Rider Settings")]
    [SerializeField] private int damage = 25;
    [SerializeField] private float attackDuration = 1.2f;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float postAttackDelay = 0.5f;

    [Header("Charge Settings")]
    [SerializeField] private float chargeDuration = 1.2f;
    [SerializeField] private float chargeStartDelay = 0.3f;
    [SerializeField] private float chargeSpeed = 4f;
    [SerializeField] private float chargeDamageMultiplier = 1.3f;
    [SerializeField] private float chargeChance = 0.3f;
    [SerializeField] private float chargeCooldown = 3f;

    // Animation states
    private enum AnimationState { Idle, Walking, Attacking, Charging }
    private AnimationState currentState = AnimationState.Idle;

    private bool isCharging = false;
    private float lastChargeTime = -10f;
    private Vector2 lastDirection;
    private bool canCharge => Time.time - lastChargeTime >= chargeCooldown;

    protected override void Start()
    {
        base.Start();
        detectionRange = 7f;
        attackRange = 1.8f;
    }

    protected override void Update()
    {
        if (target == null || GetComponent<FreezeEffect>() != null)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimationState(AnimationState.Idle);
            return;
        }

        lastDirection = (target.position - transform.position).normalized;
        UpdateSpriteFlip();

        if (!isAttacking && !isCharging)
        {
            if (IsPlayerInAttackRange())
            {
                Attack();
            }
            else if (IsPlayerInDetectionRange())
            {
                TryChargeAttack();
                HandleMovement();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                UpdateAnimationState(AnimationState.Idle);
            }
        }
    }

    private void UpdateSpriteFlip()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = lastDirection.x < 0;
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
        if (canCharge && Random.value <= chargeChance &&
            Vector2.Distance(transform.position, target.position) > attackRange * 1.5f)
        {
            StartCoroutine(ChargeRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        UpdateAnimationState(AnimationState.Attacking);

        yield return new WaitForSeconds(attackDelay);

        if (IsPlayerInAttackRange())
        {
            target?.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }

        yield return new WaitForSeconds(attackDuration - attackDelay);

        // Thời gian hồi chiêu sau tấn công
        yield return new WaitForSeconds(postAttackDelay);

        isAttacking = false;
    }

    private IEnumerator ChargeRoutine()
    {
        isCharging = true;
        lastChargeTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        UpdateAnimationState(AnimationState.Charging);

        yield return new WaitForSeconds(chargeStartDelay);

        Vector2 chargeDirection = lastDirection;
        float chargeEndTime = Time.time + (chargeDuration - chargeStartDelay);
        bool hasHit = false;

        while (Time.time < chargeEndTime)
        {
            rb.linearVelocity = chargeDirection * chargeSpeed;

            if (!hasHit && target != null &&
                Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                target.GetComponent<PlayerHealth>()?.TakeDamage(Mathf.RoundToInt(damage * chargeDamageMultiplier));
                hasHit = true;
            }

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isCharging = false;
    }

    protected override void HandleMovement()
    {
        if (isAttacking || isCharging) return;

        if (IsPlayerInDetectionRange() && !IsPlayerInAttackRange())
        {
            rb.linearVelocity = lastDirection * moveSpeed;
            UpdateAnimationState(AnimationState.Walking);
        }
    }

    protected override void UpdateAnimation()
    {
        // Đã được xử lý trong UpdateAnimationState
    }

    private void UpdateAnimationState(AnimationState newState)
    {
        if (animator == null || currentState == newState) return;

        currentState = newState;

        // Reset all parameters first
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsCharging", false);

        switch (currentState)
        {
            case AnimationState.Walking:
                animator.SetBool("IsWalking", true);
                break;
            case AnimationState.Attacking:
                animator.SetBool("IsAttacking", true);
                break;
            case AnimationState.Charging:
                animator.SetBool("IsCharging", true);
                break;
            case AnimationState.Idle:
                // All parameters already false
                break;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // Vẽ hướng di chuyển hiện tại
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)lastDirection * 2f);
        }
    }
}