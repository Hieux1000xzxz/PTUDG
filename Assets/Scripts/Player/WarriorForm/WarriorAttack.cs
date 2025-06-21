using UnityEngine;

public class WarriorAttack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackLength = 1.5f;
    public float attackWidth = 0.5f;
    public int attackDamage = 15;
    private AudioSource audioSource;
    public AudioClip attackSound; // Âm thanh tấn công
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PerformAttack()
    {
        Vector2 offset = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        Vector2 center = (Vector2)attackPoint.position + offset * (attackLength / 2f);

        Collider2D[] enemies = Physics2D.OverlapBoxAll(center, new Vector2(attackLength, attackWidth), 0f);
        audioSource.PlayOneShot(attackSound); // Phát âm thanh tấn công
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
            }
        }
    }
}
