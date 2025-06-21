using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Slider cooldownSlider;
    public float cooldownDuration = 5f;
    public KeyCode keySkill;
    public Image skillIcon; // Reference to the skill icon Image component
    public Sprite warriorSkillSprite; // Sprite for warrior form
    public Sprite mageSkillSprite; // Sprite for mage form

    private float cooldownTimer;
    private bool isCoolingDown;
    public float requiredMana = 10f;
    private PlayerHealth playerHealth;
    private PlayerController playerController;

    void Awake()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void Start()
    {
        cooldownSlider.maxValue = cooldownDuration;
        cooldownSlider.value = cooldownDuration;
        cooldownSlider.gameObject.SetActive(false);
        UpdateSkillIcon();
    }

    void Update()
    {
        if (Input.GetKeyDown(keySkill) && !isCoolingDown)
        {
            if (playerHealth != null && playerHealth.currentMana >= requiredMana)
            {
                StartCooldown();
            }
            else
            {
                Debug.Log("Không đủ mana!");
            }
        }

        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownSlider.value = cooldownDuration - cooldownTimer;

            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;
                cooldownSlider.gameObject.SetActive(false);
            }
        }
    }

    public void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownDuration;
        cooldownSlider.value = 0;
        cooldownSlider.gameObject.SetActive(true);
    }

    public void UpdateSkillIcon()
    {
        if (skillIcon == null) return;

        skillIcon.sprite = playerController.currentForm == PlayerForm.Warrior
            ? warriorSkillSprite
            : mageSkillSprite;
    }
}