using UnityEngine;

public class UndeadAI : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float maxEngagementRange = 10f;
    [SerializeField] private float summonCooldown = 12f;
    [SerializeField] private float attackCooldown = 5f;

    [Header("Skill References")]
    [SerializeField] private UndeadDashSkill dashSkill;
    [SerializeField] private UndeadAttackSkill attackSkill;
    [SerializeField] private UndeadSummonSkill summonSkill;
    [SerializeField] private Animator animator;

    private Transform target;
    private float lastSummonTime;
    private float lastAttackTime;
    private const float StopDistance = 2f;
    private const float WalkSpeed = 2f;

    private void Start()
    {
        FindPlayerTarget();
    }

    private void Update()
    {
        if (target == null)
        {
            FindPlayerTarget();
            return;
        }

        if (summonSkill.IsSummoning) return;

        float distance = Vector3.Distance(transform.position, target.position);

        // Chỉ hành động nếu player trong phạm vi chiến đấu
        if (distance > maxEngagementRange) return;

        bool didSomething = TrySummon(distance);

        if (!didSomething) didSomething = TryAttack(distance);

        if (!didSomething) didSomething = TryDash(distance);

        if (!didSomething) MoveTowardsPlayer(distance);
    }

    private bool TrySummon(float distance)
    {
        if (Time.time - lastSummonTime >= summonCooldown)
        {
            summonSkill.Execute();
            lastSummonTime = Time.time;
            return true;
        }
        return false;
    }

    private bool TryAttack(float distance)
    {
        if (!dashSkill.IsDashing && distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            attackSkill.Execute();
            lastAttackTime = Time.time;
            return true;
        }
        return false;
    }

    private bool TryDash(float distance)
    {
        if (distance > attackRange && dashSkill.CanDash())
        {
            dashSkill.Execute(target.position);
            return true;
        }
        return false;
    }

    private void MoveTowardsPlayer(float distance)
    {
        if (distance > StopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - direction * StopDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * WalkSpeed);
        }

        FlipBasedOnMovement(target.position.x);
    }

    private void FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    private void FlipBasedOnMovement(float targetXPosition)
    {
        bool shouldFlip = (targetXPosition > transform.position.x) ^ (transform.localScale.x > 0);
        if (shouldFlip) Flip();
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}