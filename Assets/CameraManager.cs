using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine; // Bỏ nếu không dùng Cinemachine

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera virtualCamera; // Gắn camera hiện tại (trong scene mới)

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm Player đang sống (DontDestroyOnLoad)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && virtualCamera != null)
        {
            Canvas canvas = player.GetComponentInChildren<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
            Debug.Log("Gắn Camera cho Player thành công sau khi load map.");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player hoặc Camera để gắn.");
        }
    }
}
