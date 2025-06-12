using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Transform bar; // chính là Fill

    private void Awake()
    {
        SetHealth(1f); // đầy máu khi khởi tạo
    }

    public void SetHealth(float healthPercent)
    {
        healthPercent = Mathf.Clamp01(healthPercent);
        bar.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}
