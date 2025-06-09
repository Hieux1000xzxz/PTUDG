using UnityEngine;
using UnityEngine.SceneManagement;

public class GateManager : MonoBehaviour
{
    [SerializeField] private GameObject gate;
    [SerializeField] private string requiredItem = "GateTicket"; // Tên item cần thiết để mở cổng
    private void Start()
    {
        if (gate != null)
            gate.SetActive(false);
    }

    private void Update()
    {
        if (gate != null && !gate.activeSelf)
        {
            if (PlayerInventory.Instance.GetItemCount(requiredItem) > 0)
            {
                gate.SetActive(true);
                Debug.Log("Gate is now active!");
            }
        }
    }

}
