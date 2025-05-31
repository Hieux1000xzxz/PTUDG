using UnityEngine;

public class MageAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    public Transform attackPoint;
    public float projectileSpeed = 8f;
    public int attackDamage = 10;
    public string enemyTag = "Enemy";

    public void PerformAttack(Vector2 _)
    {
        if (!attackPrefab) return;

        // Tìm kẻ địch gần nhất
        GameObject target = FindNearestEnemy();
        if (target == null) return;

        Vector2 direction = (target.transform.position - attackPoint.position).normalized;

        GameObject projectile = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.damage = attackDamage;
        }

        Destroy(projectile, 2f);
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        Vector2 myPos = attackPoint.position;

        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(myPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
