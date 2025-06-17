using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cài đặt Camera")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Mặc định cho 2D

    private Transform target;
    private Vector3 initialOffset;

    void Awake()
    {
        // Tìm player ngay khi scene load
        FindPlayerTarget();
        initialOffset = offset;
    }

    void FixedUpdate() // Sử dụng FixedUpdate thay vì LateUpdate khi theo dõi vật lý
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.fixedDeltaTime // Sử dụng fixedDeltaTime
            );
            transform.position = smoothedPosition;
        }
    }

    void FindPlayerTarget()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("Đã tìm thấy Player: " + playerObj.name);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy đối tượng với tag 'Player'");
            // Tự động tìm lại sau 1 giây nếu không thấy
            Invoke("FindPlayerTarget", 1f);
        }
    }

    // Gọi khi cần thay đổi target
    public void SetNewTarget(Transform newTarget)
    {
        target = newTarget;
    }
}