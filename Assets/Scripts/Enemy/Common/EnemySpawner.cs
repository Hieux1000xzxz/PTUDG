using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab quái vật
    public float spawnInterval = 5f; // Thời gian giữa các lần spawn
    public int maxEnemies = 10; // Số lượng quái tối đa
    public float spawnRadius = 5f; // Bán kính spawn ngẫu nhiên

    private int currentEnemies = 0;

    void Start()
    {
        // Bắt đầu spawn quái theo chu kỳ
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                // Tính vị trí spawn ngẫu nhiên trong bán kính
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

                // Tạo quái vật
                GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                enemy.transform.SetParent(null); // đảm bảo không nằm trong bất kỳ transform nào khác
                currentEnemies++;

                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.OnDeath += () => currentEnemies--; // Thêm sự kiện khi enemy chết
                }
                else
                {
                    Destroy(enemy, 30f);
                    currentEnemies--;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Vẽ Gizmo để dễ nhìn thấy spawn point trong editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}