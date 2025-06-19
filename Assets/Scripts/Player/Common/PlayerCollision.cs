using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sister"))
        {
            GameManager.Instance.GameWin();
            Debug.Log("Đã hoàn thành nhiệm vụ! Chúc mừng bạn đã cứu chị gái!");
        }
    }
}
