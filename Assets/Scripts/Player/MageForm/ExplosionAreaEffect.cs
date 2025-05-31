using UnityEngine;
public class ExplosionAreaEffect : MonoBehaviour
{
    private float radius;
    private int damage;
    private float delay;
    private string enemyTag;

    public void Init(float radius, int damage, float delay, string enemyTag)
    {
        this.radius = radius;
        this.damage = damage;
        this.delay = delay;
        this.enemyTag = enemyTag;
        Invoke(nameof(Explode), delay);
    }

    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                var health = hit.GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }
        Destroy(gameObject);
    }
}