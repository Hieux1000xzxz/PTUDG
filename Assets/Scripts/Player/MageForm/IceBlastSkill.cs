using System.Collections;
using UnityEngine;

public class IceBlastSkill : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject iceProjectilePrefab;
    public float projectileSpeed = 10f;
    public Vector2 explosionAreaSize = new Vector2(4f, 2f);
    public int damage = 25;
    public float freezeDuration = 2f;

    [Header("Ice Blast Settings")]
    public float cooldownTime = 8f;
    public float manaCost = 20;
    private float cooldownTimer = 0f;


    [Header("Enemy Settings")]
    public string enemyTag = "Enemy";

    private Animator animator;
    private bool isCasting = false;
    private PlayerHealth playerHealth;
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void ActivateIceBlast()
    {
      
        if (cooldownTimer > 0f) return;
        isCasting = true;
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;
        animator.SetBool("isAttacking", true);
        
        if (nearestEnemy == null) return;
        playerHealth.UseMana(manaCost); // Trừ mana
        Vector2 direction = (nearestEnemy.transform.position - transform.position).normalized;

        GameObject proj = Instantiate(iceProjectilePrefab, transform.position, Quaternion.identity);
        IceProjectile projectile = proj.GetComponent<IceProjectile>();
        if (projectile != null)
        {
            projectile.InitializeHoming(nearestEnemy, projectileSpeed, explosionAreaSize, damage, freezeDuration, enemyTag);
        }
        Invoke("ResetAnimator", 0.5f);
        cooldownTimer = cooldownTime;
        isCasting = false;
    }
    //tạo hàm reset animator sau 1 giây
    public void ResetAnimator()
    {
        animator.SetBool("isAttacking", false);
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
    public bool IsCasting()
    {
        return isCasting;
    }
}
