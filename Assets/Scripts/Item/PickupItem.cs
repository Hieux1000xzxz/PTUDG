using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private string itemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory.Instance.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}
