using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private float attractSpeed = 2f;
    [SerializeField] private float attractRange = 1f;
    [SerializeField] private float pickupDistance = 0.2f;

    private Transform player;
    private bool isAttracted = false;

    void Update()
    {
        if (player == null)
        {
            var found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
            else return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attractRange)
            isAttracted = true;

        if (isAttracted)
        {
            // Bay về phía player
            transform.position = Vector2.MoveTowards(transform.position, player.position, attractSpeed * Time.deltaTime);

            // Nếu đủ gần → thu thập
            if (distance <= pickupDistance)
            {
                PlayerInventory.Instance.AddItem(itemName);
                Destroy(gameObject);
            }
        }
    }
}
