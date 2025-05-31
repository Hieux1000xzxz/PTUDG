using System.Collections;
using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    private GameObject targetEnemy;
    private float speed;
    private Vector2 areaSize;
    private int damage;
    private float freezeDuration;
    private string enemyTag;

    public GameObject iceExplosionEffectPrefab;

    private Vector2 direction;

    public void InitializeHoming(GameObject target, float spd, Vector2 area, int dmg, float freezeTime, string tag)
    {
        targetEnemy = target;
        speed = spd;
        areaSize = area;
        damage = dmg;
        freezeDuration = freezeTime;
        enemyTag = tag;

        if (targetEnemy != null)
        {
            direction = ((Vector2)targetEnemy.transform.position - (Vector2)transform.position).normalized;
        }

        StartCoroutine(FlyForward());
    }

    private IEnumerator FlyForward()
    {
        float lifetime = 5f;
        float timer = 0f;

        while (timer < lifetime)
        {
            if (targetEnemy != null)
            {
                direction = ((Vector2)targetEnemy.transform.position - (Vector2)transform.position).normalized;
            }

            transform.Translate(direction * speed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(enemyTag))
        {
            ExplodeAt(transform.position);
            Destroy(gameObject);
        }
    }

    private void ExplodeAt(Vector2 center)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, areaSize, 0f);
        foreach (var enemy in hits)
        {
            if (enemy.CompareTag(enemyTag))
            {
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(damage);
                enemy.gameObject.AddComponent<FreezeEffect>().Freeze(freezeDuration);
            }
        }

        if (iceExplosionEffectPrefab != null)
        {
            GameObject fx = Instantiate(iceExplosionEffectPrefab, center, Quaternion.identity);
            Destroy(fx, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
