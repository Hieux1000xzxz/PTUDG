using UnityEngine;
using UnityEngine.SceneManagement;

public class GateTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private string requiredItem = "Gate Ticket";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerInventory.Instance.GetItemCount(requiredItem) > 0)
            {
                PlayerInventory.Instance.RemoveItem(requiredItem);
                Debug.Log("Player passed the gate!");

                // Đăng ký hàm khi scene load xong
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("You need a gate ticket to pass!");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Di chuyển player tới điểm spawn
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        // Hủy đăng ký sau khi dùng xong
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
