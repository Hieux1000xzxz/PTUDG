using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 30f;
    [SerializeField] private float lifetime = 1f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(int damage, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = damage.ToString();
            textMesh.color = color;
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (rectTransform != null)
        {
            rectTransform.position += Vector3.up * floatSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        }
    }
}
