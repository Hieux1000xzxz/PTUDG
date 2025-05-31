using UnityEngine;

public class MageExplosionSkill : MonoBehaviour
{
    public GameObject explosionAreaPrefab; // Prefab có SpriteRenderer hình tròn đỏ
    public float explosionRadius = 2.5f;
    public int explosionDamage = 30;
    public float explosionDelay = 2f;
    public string enemyTag = "Enemy";

    public void ActivateExplosion()
    { 
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        Vector2 targetPos = nearestEnemy.transform.position;
        GameObject area = Instantiate(explosionAreaPrefab, targetPos, Quaternion.identity);

        // Hiển thị vùng nổ (chỉ màu đỏ, không gây damage ngay)
        SpriteRenderer sr = area.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1, 0, 0, 0.3f); // đỏ mờ
            float scale = explosionRadius * 2f / sr.sprite.bounds.size.x;
            area.transform.localScale = new Vector3(scale, scale, 1);
        }

        // Sau delay, gây damage và hủy vùng nổ
        area.AddComponent<ExplosionAreaEffect>().Init(explosionRadius, explosionDamage, explosionDelay, enemyTag);
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        Vector2 myPos = transform.position;
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