using UnityEngine;
using UnityEngine.SceneManagement;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private string requiredItem = "GateTicket";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerInventory.Instance.GetItemCount(requiredItem) > 0)
            {
                PlayerInventory.Instance.RemoveItem(requiredItem);
                Debug.Log("Player passed the gate!");
                SceneManager.LoadScene(nextSceneName);
                GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
                if (spawnPoint != null)
                {
                    transform.position = spawnPoint.transform.position;
                }
            }
            else
            {
                Debug.Log("You need a gate ticket to pass!");
            }
        }
    }
}
