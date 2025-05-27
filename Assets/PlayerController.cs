using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackLength = 1.5f;
    public float attackWidth = 0.4f;
    public float attackDelay = 0.6f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayers;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right; // vẫn dùng để flip sprite

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 attackLockPosition;

    private bool isAttacking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isAttacking) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Update last move direction for flipping sprite only
        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

        // Flip sprite left/right
        if (lastMoveDirection.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (lastMoveDirection.x < -0.1f)
            spriteRenderer.flipX = true;

        // Animation
        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0);

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(Attack());
        }
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            // Giữ nguyên vị trí trong lúc tấn công
            rb.MovePosition(attackLockPosition);
        }
        else
        {
            Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        attackLockPosition = rb.position; // Lưu vị trí hiện tại

        yield return new WaitForSeconds(attackDelay); // Đợi animation

        // Tính vị trí vùng attack theo hướng trái/phải của sprite
        Vector2 offset = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 center = (Vector2)attackPoint.position + offset * (attackLength / 2f);
        float angle = 0f; // không xoay

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            center,
            new Vector2(attackLength, attackWidth),
            angle,
            enemyLayers
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>()?.TakeDamage(15);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;

        // Lấy component SpriteRenderer tại runtime (trong editor có thể null, nên kiểm tra)
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        Vector2 offset = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 pos = (Vector2)attackPoint.position + offset * (attackLength / 2f);
        float angle = 0f;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackLength, attackWidth, 0));
    }
}
