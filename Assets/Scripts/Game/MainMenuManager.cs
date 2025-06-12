using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    
    public void PlayGame()
    {
   
        SceneManager.LoadScene("Map1"); // Assuming "GameScene" is the name of your game scene
    }
    public void QuitGame()
    {
        
        Application.Quit();
    }
}
