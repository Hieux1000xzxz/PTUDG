using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 2f;

    protected Transform target;
    protected float lastAttackTime;
    protected bool isAttacking = false;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (target == null) return;

        HandleMovement();
        HandleAttack();
        UpdateAnimation();
    }

    protected virtual void HandleMovement()
    {
        if (isAttacking) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // Flip sprite theo hướng player
        spriteRenderer.flipX = target.position.x < transform.position.x;

        if (distance <= detectionRange && distance > attackRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            Move(direction);
        }
    }

    protected virtual void Move(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    protected virtual void HandleAttack()
    {
        if (isAttacking || !IsPlayerInAttackRange()) return;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    protected bool IsPlayerInAttackRange()
    {
        if(target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= attackRange;
    }
    protected bool IsPlayerInDetectionRange()
    {
        if (target == null) return false;
        return Vector2.Distance(transform.position, target.position) <= detectionRange;
    }

    protected abstract void Attack();
    protected abstract void UpdateAnimation();

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}