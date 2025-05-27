﻿using System.Collections;
using UnityEngine;

public class SummonAI : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 5f;
    public float stopDistance = 2f;
    public float attackSpeed = 10f;
    public float waitDuration = 1f;
    public float retreatDistance = 2f;
    public float postRetreatPause = 1f;

    public Animator animator;
    public float spawnDelay = 1f;

    [Header("Combat Settings")]
    public int damage = 10;
    public float attackCooldown = 1f; // Thời gian giữa 2 lần gây damage
    public LayerMask playerLayer;
    public float attackRange = 0.5f;  // Phạm vi kiểm tra trúng player khi charge
    private bool isWaiting = false;
    private bool isCharging = false;
    private bool isActive = false;
    private float lastAttackTime = 0f;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        animator.Play("SummonAppear");
        StartCoroutine(ActivateAfterSpawn());
      
    }
   
    private IEnumerator ActivateAfterSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        isActive = true;
    }

    private void Update()
    {
        if (!isActive || target == null) return;

        animator.Play("SummonIdle");

        float distance = Vector3.Distance(transform.position, target.position);

        if (!isCharging && distance > stopDistance)
        {
            MoveTowards(target.position, moveSpeed);
        }
        else if (!isCharging && !isWaiting)
        {
            StartCoroutine(WaitBeforeCharge());
        }
        else if (isCharging)
        {
            MoveTowards(target.position, attackSpeed);

            // Khi charge, kiểm tra va chạm để gây damage
            TryDamagePlayer();

            if (distance <= 0.5f)
            {
                isCharging = false;
                StartCoroutine(RetreatAndPause());
            }
        }
    }

    private void MoveTowards(Vector3 destination, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    private IEnumerator WaitBeforeCharge()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitDuration);
        isWaiting = false;
        isCharging = true;
    }

    private IEnumerator RetreatAndPause()
    {
        Vector3 retreatDir = (transform.position - target.position).normalized;
        Vector3 retreatPos = transform.position + retreatDir * retreatDistance;

        float retreatSpeed = attackSpeed * 0.8f;
        while (Vector3.Distance(transform.position, retreatPos) > 0.05f)
        {
            MoveTowards(retreatPos, retreatSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(postRetreatPause);

        isWaiting = false;
        isCharging = false;
    }

    private void TryDamagePlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return; // Nếu đang cooldown, bỏ qua

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        /*foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }*/
    }


    private void OnDrawGizmosSelected()
    {
        // Vẽ attack range để dễ debug
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
