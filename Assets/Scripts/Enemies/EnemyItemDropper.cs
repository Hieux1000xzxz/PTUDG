using UnityEngine;

public class EnemyItemDropper : MonoBehaviour
{
    [Header("Potion Drop Settings")]
    [SerializeField] private GameObject healthPotionSmall;
    [SerializeField] private GameObject healthPotionMedium;
    [SerializeField] private GameObject healthPotionLarge;
    [SerializeField] private GameObject manaPotionSmall;
    [SerializeField] private GameObject manaPotionMedium;
    [SerializeField] private GameObject manaPotionLarge;

    [Header("Bonus Drop Settings")]
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject crystalPrefab;

    [Header("Special Drop")]
    [SerializeField] private GameObject gateTicketPrefab; // Vé qua cổng

    public void DropItems()
    {
        Vector3 dropPosition = transform.position;

        DropOnePotion(dropPosition);
        TrySpawn(goldPrefab, 50f, dropPosition);     // 50% rơi vàng
        TrySpawn(crystalPrefab, 5f, dropPosition);   // 5% rơi tinh thạch

        // Vé qua cổng: nếu được gán thì luôn rơi
        if (gateTicketPrefab != null)
        {
            SpawnItem(gateTicketPrefab, dropPosition);
        }
    }

    private void DropOnePotion(Vector3 position)
    {
        float roll = Random.Range(0f, 100f);

        if (roll < 20f)
            SpawnItem(healthPotionSmall, position);
        else if (roll < 30f)
            SpawnItem(healthPotionMedium, position);
        else if (roll < 40f)
            SpawnItem(healthPotionLarge, position);
        else if (roll < 50f)
            SpawnItem(manaPotionSmall, position);
        else if (roll < 60f)
            SpawnItem(manaPotionMedium, position);
        else if (roll < 70f)
            SpawnItem(manaPotionLarge, position);
        // else: không rơi gì
    }

    private void TrySpawn(GameObject prefab, float chancePercent, Vector3 position)
    {
        if (prefab == null) return;

        float roll = Random.Range(0f, 100f);
        if (roll < chancePercent)
        {
            SpawnItem(prefab, position);
        }
    }

    private void SpawnItem(GameObject prefab, Vector3 centerPosition)
    {
        if (prefab == null) return;

        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(0.2f, 0.6f);
        Vector3 spawnPos = centerPosition + (Vector3)offset;

        GameObject item = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Thêm lực bay nhẹ nếu có Rigidbody2D
        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 force = offset.normalized * Random.Range(1.5f, 3f);
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
