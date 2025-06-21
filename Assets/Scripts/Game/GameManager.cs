using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private GameObject PauseMenuUI;
    [SerializeField] private GameObject GameWinUI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ lại GameManager khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        GameOverUI.SetActive(false);
        PauseMenuUI.SetActive(false);
        GameWinUI.SetActive(false);
    }
    
    public void OpenMenu()
    {
        Time.timeScale = 0f; // Dừng thời gian
        PauseMenuUI.SetActive(true);
    }
    public void CloseMenu()
    {
        Time.timeScale = 1f; // Tiếp tục thời gian
        PauseMenuUI.SetActive(false);
    }
    public void GameOver()
    {
        //đợi 2 giây
        Time.timeScale = 0f; // Dừng thời gian
        GameOverUI.SetActive(true);
    }
    public void ReloadScene()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian được tiếp tục
        GameOverUI.SetActive(false); // Ẩn GameOver UI
        SceneManager.LoadScene("Map1"); // Tải lại scene hiện tại
    }
    public void GameWin()
    {
        Time.timeScale = 0f; // Dừng thời gian
        GameWinUI.SetActive(true);
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f; // Đảm bảo thời gian được tiếp tục
        GameOverUI.SetActive(false); // Ẩn GameOver UI
        GameWinUI.SetActive(false); // Ẩn GameWin UI
        PauseMenuUI.SetActive(false); // Ẩn Pause Menu UI
        //xóa người chơi 
        if (FindAnyObjectByType<PlayerHealth>() != null)
        {
            PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
            Destroy(playerHealth.gameObject);
        }
        SceneManager.LoadScene("MainMenu"); // Quay về menu chính
    }
 
}
