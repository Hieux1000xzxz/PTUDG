using UnityEngine;

public class MageAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    public Transform attackPoint;
    public float projectileSpeed = 8f;
    public int attackDamage = 10;

    public void PerformAttack(Vector2 direction)
    {
        if (!attackPrefab) return;

        GameObject projectile = Instantiate(attackPrefab, attackPoint.position + new Vector3 (0,-0.2f,0), Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * projectileSpeed;
        }

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.damage = attackDamage;
        }

        Destroy(projectile, 2f);
    }
}
