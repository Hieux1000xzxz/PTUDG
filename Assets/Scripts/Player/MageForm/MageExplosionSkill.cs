using System.Collections;
using UnityEngine;

public class MageExplosionSkill : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionAreaPrefab;
    public GameObject explosionEffect;
    public float explosionRadius = 2.5f;
    public int explosionDamage = 30;
    public float explosionDelay = 2f;
    public string enemyTag = "Enemy";
    public float manaCost = 20f;

    [Header("Cooldown Settings")]
    public float cooldownTime = 10f;
    private float cooldownTimer = 0f;

    private bool isExploding = false;
    private Animator animator;
    private PlayerHealth playerHealth;
    private Vector2 lockPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }
    public Vector2 GetLockPosition()
    {
        return lockPosition;
    }
    public void ActivateExplosion()
    {
        if (isExploding) return;             // Đang thực hiện skill
        if (cooldownTimer > 0f) return;           // Đang cooldown
        if (playerHealth == null) return;
        if (playerHealth.currentMana < manaCost)
        {
            Debug.Log("Không đủ mana để dùng Triple Slash");
            return;
        }

        playerHealth.UseMana(manaCost); // Trừ mana
        cooldownTimer = cooldownTime;
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;
        StartCoroutine(ExplosionCoroutine(nearestEnemy.transform.position));
       
    }

    private IEnumerator ExplosionCoroutine(Vector2 targetPos)
    {
        if(targetPos == null)
        {
            Debug.LogWarning("Target position is null, cannot perform explosion.");
            yield break;
        }
        isExploding = true;
        animator.SetBool("isUsingSkill1", true);
        lockPosition = transform.position;
        yield return new WaitForSeconds(0.1f);
        GameObject area = Instantiate(explosionAreaPrefab, targetPos, Quaternion.identity);
        SpriteRenderer sr = area.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1, 0, 0, 0.2f);
            float scale = explosionRadius * 2f / sr.sprite.bounds.size.x;
            area.transform.localScale = new Vector3(scale, scale, 1);
        }

        yield return new WaitForSeconds(explosionDelay);

        DealExplosionDamage(targetPos);
        GameObject effect = Instantiate(explosionEffect, targetPos, Quaternion.identity);
        Destroy(area);

        animator.SetBool("isUsingSkill1", false);
        isExploding = false;
        yield return new WaitForSeconds(0.8f);
        Destroy(effect);
    }

    private void DealExplosionDamage(Vector2 position)
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag(enemyTag))
            {
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(explosionDamage);
            }
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public bool IsExploding()
    {
        return isExploding;
    }
}
