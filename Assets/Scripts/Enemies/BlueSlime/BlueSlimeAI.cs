using System.Collections;
using UnityEngine;

public class BlueSlimeAi : MonoBehaviour
{
    private enum State
    {
        Roaming
    }

    private State state;
    private BlueSlimePathFinding pathfinding;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private int damage = 10;

    private float attackCooldown = 0f;

    private void Awake()
    {
        pathfinding = GetComponent<BlueSlimePathFinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;
        TryAttackNearbyPlayer();
    }

    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                Vector2 roamPosition = GetRoamingPosition();
                pathfinding.MoveTo(roamPosition);
            }
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void TryAttackNearbyPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && attackCooldown <= 0f)
            {
                AttackPlayer(hit.gameObject);
                attackCooldown = attackRate;
                break;
            }
        }
    }

    private void AttackPlayer(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
