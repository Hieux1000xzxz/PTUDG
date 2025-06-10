using System.Collections;
using UnityEngine;

public class SkeletonAI : EnemyAI
{
    [Header("Skeleton Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        if (animator != null) animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(1f);

        ShootArrow();
        if (animator != null) animator.SetBool("isAttacking", false);

        isAttacking = false;
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        Vector2 direction = (target.position - firePoint.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.SetDirection(direction);
        }
    }

    protected override void UpdateAnimation()
    {
        if (animator == null) return;

        bool shouldWalk = !isAttacking &&
                         IsPlayerInDetectionRange() &&
                         !IsPlayerInAttackRange();

        animator.SetBool("isWalking", shouldWalk);
    }
}