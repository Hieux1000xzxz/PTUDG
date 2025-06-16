using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FormChangeEffect : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D playerLight;
    [SerializeField] private float lightIntensity = 3f;
    [SerializeField] private Color warriorColor = Color.red;
    [SerializeField] private Color mageColor = Color.blue;

    [Header("Vignette Settings")]
    [SerializeField] private Volume volume;
    [SerializeField] private float intensity = 0.4f;
    [SerializeField] private float effectDuration = 1f;

    private Bloom bloom;
    private PlayerForm currentForm;

    private void Awake()
    {
        // Tự động thêm Light2D nếu chưa có
        if (playerLight == null)
        {
            playerLight = GetComponentInChildren<Light2D>();
            if (playerLight == null)
            {
                GameObject lightObj = new GameObject("PlayerLight");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.zero;
                playerLight = lightObj.AddComponent<Light2D>();
                playerLight.lightType = Light2D.LightType.Point;
                playerLight.pointLightOuterRadius = 3f;
                playerLight.intensity = 0f;
            }
        }

        // Lấy Vignette từ Volume
        if (volume != null && volume.profile.TryGet(out bloom))
        {
            bloom.intensity.value = 0.5f;
        }
    }

    public void PlayEffect(PlayerForm newForm)
    {
        currentForm = newForm;
        StopAllCoroutines(); // Dừng hiệu ứng cũ nếu đang chạy

        // Áp dụng màu sắc theo form
        Color lightColor = (currentForm == PlayerForm.Warrior) ? warriorColor : mageColor;

        // Kích hoạt hiệu ứng
        StartCoroutine(GlowLightEffect(lightColor));
        StartCoroutine(BloomEffect());
    }

    private IEnumerator GlowLightEffect(Color targetColor)
    {
        if (playerLight == null) yield break;

        playerLight.color = targetColor;
        float elapsed = 0f;

        // Sáng dần
        while (elapsed < effectDuration / 2)
        {
            playerLight.intensity = Mathf.Lerp(0f, lightIntensity, elapsed / (effectDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Tắt dần
        elapsed = 0f;
        while (elapsed < effectDuration / 2)
        {
            playerLight.intensity = Mathf.Lerp(lightIntensity, 0f, elapsed / (effectDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerLight.intensity = 0f;
    }

    private IEnumerator BloomEffect()
    {
        if (bloom == null) yield break;

        // Sáng ngay lập tức
        bloom.intensity.value = intensity;

        // Giữ trong một khoảng thời gian
        yield return new WaitForSeconds(effectDuration);

        // Tắt hiệu ứng
        bloom.intensity.value = 0f;
    }
}