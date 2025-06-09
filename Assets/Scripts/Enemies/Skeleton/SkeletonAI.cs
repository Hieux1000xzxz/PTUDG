using System.Collections;
using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Detection & Attack")]
    public float detectionRange = 10f;
    public float attackRange = 6f;
    public float moveSpeed = 2f;
    public float attackCooldown = 2f;
    public GameObject arrowPrefab;
    public Transform firePoint;
    
    private Transform target;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isWalking = false;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (target == null || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        // Lật hướng
        spriteRenderer.flipX = target.position.x < transform.position.x;

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            if (distanceToPlayer > attackRange)
            {
                // Đuổi theo người chơi
                transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
                isWalking = true;
            }
            else
            {
                isWalking = false;
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    StartCoroutine(AttackWithDelay(target.position));
                    lastAttackTime = Time.time;
                }
            }
        }
        else
        {
            isWalking = false;
        }

        animator.SetBool("isWalking", isWalking);
    }

    private IEnumerator AttackWithDelay(Vector2 lockedTargetPos)
    {
        isAttacking = true;
        animator.SetBool("isAttacking",true);
        // Rặn 1 giây
        yield return new WaitForSeconds(1f);
        animator.SetBool("isAttacking", false);
        Vector2 shootDir = (lockedTargetPos - (Vector2)firePoint.position).normalized;
        ShootArrow(shootDir);

        isAttacking = false;
    }

    private void ShootArrow(Vector2 direction)
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
