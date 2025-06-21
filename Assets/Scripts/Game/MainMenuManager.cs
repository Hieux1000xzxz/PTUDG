using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Update()
    {
        // Kiểm tra nếu có nhấn phím Escape để thoát game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }
    public void PlayGame()
    {
        InventoryUIManager inventoryUIManager = FindAnyObjectByType<InventoryUIManager>();
        if (inventoryUIManager != null)
        {
            inventoryUIManager.gameObject.SetActive(true); // Đóng Inventory nếu đang mở
        }
        SceneManager.LoadScene("Map1"); // Assuming "GameScene" is the name of your game scene
    }
    public void QuitGame()
    {
        
        Application.Quit();
    }
}
