using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("UI Components")]
    public Slider warriorSlider;
    public Slider mageSlider;
    public Image skillIcon;

    [Header("Sprites")]
    public Sprite warriorSprite;
    public Sprite mageSprite;

    [Header("Settings")]
    public float cooldownDuration = 5f;
    public KeyCode keySkill = KeyCode.Q;
    public float requiredMana = 10f;

    [Header("Visual Settings")]
    [Range(0f, 1f)]
    public float visibleAlpha = 0.6f;
    [Range(0f, 1f)]
    public float hiddenAlpha = 0.1f;

    private PlayerHealth playerHealth;
    private PlayerController playerController;

    // Cooldown riêng biệt cho từng form
    private bool warriorCoolingDown = false;
    private float warriorCooldownTimer = 0f;
    private bool mageCoolingDown = false;
    private float mageCooldownTimer = 0f;

    private PlayerForm lastForm;

    // Cache graphics để tối ưu performance
    private Graphic[] warriorGraphics;
    private Graphic[] mageGraphics;

    void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerController = FindAnyObjectByType<PlayerController>();

        // Cache graphics components một lần duy nhất
        if (warriorSlider != null)
            warriorGraphics = warriorSlider.GetComponentsInChildren<Graphic>(true);
        if (mageSlider != null)
            mageGraphics = mageSlider.GetComponentsInChildren<Graphic>(true);
    }

    void Start()
    {
        // Khởi tạo slider
        if (warriorSlider != null)
        {
            warriorSlider.maxValue = cooldownDuration;
            warriorSlider.value = cooldownDuration;
        }

        if (mageSlider != null)
        {
            mageSlider.maxValue = cooldownDuration;
            mageSlider.value = cooldownDuration;
        }

        // Lưu form hiện tại và cập nhật UI
        if (playerController != null)
        {
            lastForm = playerController.currentForm;
            UpdateFormDisplay();
        }
    }

    void Update()
    {
        // Kiểm tra thay đổi form
        if (playerController != null && playerController.currentForm != lastForm)
        {
            lastForm = playerController.currentForm;
            UpdateFormDisplay();
        }

        // Xử lý input skill
        if (Input.GetKeyDown(keySkill) && CanUseCurrentSkill())
        {
            if (playerHealth != null && playerHealth.currentMana >= requiredMana)
            {
                UseCurrentSkill();
            }
            else
            {
                Debug.Log("Không đủ mana");
            }
        }

        // Cập nhật cooldown cho cả 2 form (chạy song song)
        UpdateWarriorCooldown();
        UpdateMageCooldown();
    }

    void UpdateWarriorCooldown()
    {
        if (!warriorCoolingDown) return;

        warriorCooldownTimer -= Time.deltaTime;
        float progressValue = cooldownDuration - warriorCooldownTimer;

        if (warriorSlider != null)
            warriorSlider.value = progressValue;

        if (warriorCooldownTimer <= 0f)
        {
            warriorCoolingDown = false;
            // Chỉ ẩn slider nếu đang ở form warrior
            if (playerController != null && playerController.currentForm == PlayerForm.Warrior)
            {
                SetSliderVisible(warriorSlider, false);
            }
        }
    }

    void UpdateMageCooldown()
    {
        if (!mageCoolingDown) return;

        mageCooldownTimer -= Time.deltaTime;
        float progressValue = cooldownDuration - mageCooldownTimer;

        if (mageSlider != null)
            mageSlider.value = progressValue;

        if (mageCooldownTimer <= 0f)
        {
            mageCoolingDown = false;
            // Chỉ ẩn slider nếu đang ở form mage
            if (playerController != null && playerController.currentForm == PlayerForm.Mage)
            {
                SetSliderVisible(mageSlider, false);
            }
        }
    }

    bool CanUseCurrentSkill()
    {
        if (playerController == null) return false;

        return playerController.currentForm == PlayerForm.Warrior
            ? !warriorCoolingDown
            : !mageCoolingDown;
    }

    void UseCurrentSkill()
    {
        if (playerController == null) return;

        if (playerController.currentForm == PlayerForm.Warrior)
        {
            StartWarriorCooldown();
        }
        else
        {
            StartMageCooldown();
        }
    }

    void StartWarriorCooldown()
    {
        warriorCoolingDown = true;
        warriorCooldownTimer = cooldownDuration;

        if (warriorSlider != null)
        {
            warriorSlider.value = 0;
            SetSliderVisible(warriorSlider, true);
        }
    }

    void StartMageCooldown()
    {
        mageCoolingDown = true;
        mageCooldownTimer = cooldownDuration;

        if (mageSlider != null)
        {
            mageSlider.value = 0;
            SetSliderVisible(mageSlider, true);
        }
    }

    void UpdateFormDisplay()
    {
        UpdateSkillIcon();

        if (playerController == null) return;

        if (playerController.currentForm == PlayerForm.Warrior)
        {
            // Hiển thị warrior slider nếu đang cooldown, ẩn mage slider
            SetSliderVisible(warriorSlider, warriorCoolingDown);
            SetSliderVisible(mageSlider, false);
        }
        else
        {
            // Hiển thị mage slider nếu đang cooldown, ẩn warrior slider
            SetSliderVisible(mageSlider, mageCoolingDown);
            SetSliderVisible(warriorSlider, false);
        }
    }

    public void UpdateSkillIcon()
    {
        if (skillIcon == null || playerController == null) return;

        skillIcon.sprite = playerController.currentForm == PlayerForm.Warrior
            ? warriorSprite
            : mageSprite;
    }

    void SetSliderVisible(Slider slider, bool visible)
    {
        if (slider == null) return;

        Graphic[] graphics = slider == warriorSlider ? warriorGraphics : mageGraphics;
        if (graphics == null) return;

        float targetAlpha = visible ? visibleAlpha : hiddenAlpha;

        foreach (var graphic in graphics)
        {
            if (graphic != null)
            {
                Color color = graphic.color;
                color.a = targetAlpha;
                graphic.color = color;
            }
        }
    }

    // Utility methods
    public bool IsWarriorOnCooldown() => warriorCoolingDown;
    public bool IsMageOnCooldown() => mageCoolingDown;
    public bool IsCurrentFormOnCooldown() => playerController != null &&
        (playerController.currentForm == PlayerForm.Warrior ? warriorCoolingDown : mageCoolingDown);
}