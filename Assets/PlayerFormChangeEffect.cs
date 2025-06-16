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
    [SerializeField] private float vignetteIntensity = 0.4f;
    [SerializeField] private float effectDuration = 1f;

    private Vignette vignette;
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
        if (volume != null && volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = 0.5f;
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
        StartCoroutine(VignetteEffect());
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

    private IEnumerator VignetteEffect()
    {
        if (vignette == null) yield break;

        float elapsed = 0f;

        // Tối dần
        while (elapsed < effectDuration / 2)
        {
            vignette.intensity.value = Mathf.Lerp(0f, vignetteIntensity, elapsed / (effectDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Sáng lại
        elapsed = 0f;
        while (elapsed < effectDuration / 2)
        {
            vignette.intensity.value = Mathf.Lerp(vignetteIntensity, 0f, elapsed / (effectDuration / 2));
            elapsed += Time.deltaTime;
            yield return null;
        }

        vignette.intensity.value = 0f;
    }
}