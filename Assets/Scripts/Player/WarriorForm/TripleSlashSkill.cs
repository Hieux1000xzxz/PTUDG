using System.Collections;
using UnityEngine;

public class TripleSlashSkill : MonoBehaviour
{
    [Header("Triple Slash Settings")]
    public Transform attackPoint;
    public float attackLength = 1.5f;
    public float attackWidth = 1f;
    public float totalDuration = 1f;
    public int slashCount = 3;
    public int damagePerHit = 17;

    [Header("Cooldown Settings")]
    public float cooldownTime = 10f;
    private float cooldownTimer = 0f;

    [Header("Slash Effect")]
    public GameObject slashPrefab;          // Prefab hiệu ứng slash bay ra
    public float slashSpeed = 8f;           // Tốc độ bay ra của slash

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isTripleSlashing = false;
    private Vector2 lockPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void ActivateTripleSlash()
    {
        if (!isTripleSlashing && cooldownTimer <= 0f)
        {
            StartCoroutine(TripleSlashCoroutine());
            cooldownTimer = cooldownTime;
        }
    }

    public Vector2 GetLockPosition()
    {
        return lockPosition;
    }

    private IEnumerator TripleSlashCoroutine()
    {
        isTripleSlashing = true;
        float delay = totalDuration / slashCount;

        lockPosition = rb.position;
        animator.SetBool("isUsingSkill1", true);
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < slashCount; i++)
        {
            rb.MovePosition(lockPosition); // Giữ nguyên vị trí

            Vector2 offset = spriteRenderer.flipX ? Vector2.left : Vector2.right;
            Vector2 center = (Vector2)attackPoint.position + offset * (attackLength / 2f);

            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
                center,
                new Vector2(attackLength, attackWidth),
                0f
            );

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<EnemyHealth>()?.TakeDamage(damagePerHit);
                }
            }

            if (slashPrefab)
            {
                GameObject slash = Instantiate(slashPrefab, attackPoint.position, Quaternion.identity);

                // Lật theo hướng của người chơi
                slash.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

                // Tạo slash bay ngang
                Vector2 slashDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;

                SlashMover mover = slash.GetComponent<SlashMover>();
                if (mover != null)
                {
                    mover.direction = slashDir;
                    mover.speed = slashSpeed;
                }

                Destroy(slash, 0.5f);
            }

            yield return new WaitForSeconds(delay);
        }

        animator.SetBool("isUsingSkill1", false);
        isTripleSlashing = false;
    }

    public bool IsTripleSlashing()
    {
        return isTripleSlashing;
    }

    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 offset = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 pos = (Vector2)attackPoint.position + offset * (attackLength / 2f);

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(attackLength, attackWidth, 0));
    }
}
