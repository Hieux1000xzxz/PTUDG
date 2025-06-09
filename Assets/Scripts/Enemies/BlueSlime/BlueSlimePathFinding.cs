using UnityEngine;

public class BlueSlimePathFinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private KnockBack knockBack;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockBack = GetComponent<KnockBack>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // Không di chuyển nếu đang bị knockback
        if (knockBack != null && knockBack.isKnockedBack)
        {
            return;
        }
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

        // Lật sprite theo hướng di chuyển
        if (moveDir.x < 0)
        {
            spriteRenderer.flipX = true; // Lật sang trái
        }
        else if (moveDir.x > 0)
        {
            spriteRenderer.flipX = false; // Lật sang phải
        }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
}
