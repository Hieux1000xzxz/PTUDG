using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework;
using System.Collections.Generic;

public enum PlayerForm { Warrior, Mage }

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public float attackDelay = 0.4f;
    public float attackCooldown = 0.3f;
    public float hitCooldown = 0.35f; // Cooldown after being hit
    public AudioClip attackSound;
    [Header("Form Change Effect")]
    private FormChangeEffect formChangeEffect;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;
    public AudioClip changeSound;
    public SkillCooldownUI[] skillCooldownUIs; // Array of all skill UI elements

    [Header("Form Change Settings")]
    public float formChangeCooldown = 1.5f; // Thời gian delay giữa các lần biến hình
    private bool isFormChangeOnCooldown = false;

    // Component references
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerHealth playerHealth;

    // Attack components
    private WarriorAttack warriorAttack;
    private MageAttack mageAttack;

    // Skill components
    private TripleSlashSkill tripleSlashSkill;
    private MoonveilDashSkill moonveilDashSkill;
    private MageExplosionSkill explosionSkill;
    private IceBlastSkill iceBlastSkill;
   
    // State variables
    private bool isAttacking = false;
    private bool isMovementLocked = false;
    private bool isUsingSkill = false;
    private bool isOnHitCooldown = false;
    private bool hasMagicStaff = false;
    public bool canAttack = true; // Biến này để kiểm soát việc có thể tấn công hay không
    private Vector2 attackLockPosition;
    
    public PlayerForm currentForm;
    private AudioSource audioSource;
    [Header("Animators")]
    public RuntimeAnimatorController warriorAnimator;
    public RuntimeAnimatorController mageAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        // Get attack components
        warriorAttack = GetComponent<WarriorAttack>();
        mageAttack = GetComponent<MageAttack>();

        // Get skill components
        tripleSlashSkill = GetComponent<TripleSlashSkill>();
        moonveilDashSkill = GetComponent<MoonveilDashSkill>();
        explosionSkill = GetComponent<MageExplosionSkill>();
        iceBlastSkill = GetComponent<IceBlastSkill>();
        audioSource = GetComponent<AudioSource>();
        if (formChangeEffect == null)
        {
            formChangeEffect = GetComponent<FormChangeEffect>();
        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            // Đánh dấu GameObject này không bị hủy khi load scene mới
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        SetForm(PlayerForm.Warrior);
        
    }

    private void Update()
    {
        if (!isMovementLocked)
        {
            HandleMovementInput();
        }

        if (!isUsingSkill && !isOnHitCooldown && canAttack == true)
        {
            HandleAttackInput();
            HandleSkillInput();
        }

        HandleFormChangeInput();
        SetMagicStaff();
    }

    private void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveInput;
        }

        UpdateSpriteFlip();
        animator.SetBool("isMoving", moveInput.sqrMagnitude > 0);
    }

    private void UpdateSpriteFlip()
    {
        if (lastMoveDirection.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (lastMoveDirection.x < -0.1f)
            spriteRenderer.flipX = true;
    }

    private void HandleAttackInput()
    {
        if(canAttack == false)
        {
            return; // Không cho phép tấn công nếu không thể tấn công
        }
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void HandleSkillInput()
    {
       
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentForm == PlayerForm.Warrior && tripleSlashSkill && !tripleSlashSkill.IsTripleSlashing())
            {
                StartCoroutine(UseSkillCoroutine(tripleSlashSkill.totalDuration));
                tripleSlashSkill.ActivateTripleSlash();
            }
            else if (currentForm == PlayerForm.Mage && explosionSkill && !explosionSkill.IsExploding())
            {
                StartCoroutine(UseSkillCoroutine(explosionSkill.explosionDelay));
                explosionSkill.ActivateExplosion();
            }
          
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentForm == PlayerForm.Warrior && moonveilDashSkill && !moonveilDashSkill.IsDashing())
            {
                StartCoroutine(UseSkillCoroutine(moonveilDashSkill.dashDuration));
                moonveilDashSkill.ActivateMoonveilDash();
            }
            else if (currentForm == PlayerForm.Mage && iceBlastSkill && !iceBlastSkill.IsCasting())
            {
                StartCoroutine(UseSkillCoroutine(0.5f));
                iceBlastSkill.ActivateIceBlast();
            }
        }
    }

    private void HandleFormChangeInput()
    {
        if (isFormChangeOnCooldown) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentForm != PlayerForm.Warrior)
            {
                audioSource.PlayOneShot(changeSound);
                SetForm(PlayerForm.Warrior);
                StartCoroutine(FormChangeCooldownRoutine());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentForm != PlayerForm.Mage && hasMagicStaff)
            {
                SetForm(PlayerForm.Mage);
                StartCoroutine(FormChangeCooldownRoutine());
                audioSource.PlayOneShot(changeSound);
            }
            else if (!hasMagicStaff)
            {
                Debug.Log("You need to find the magic staff first!");
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.MovePosition(attackLockPosition);
        }
        else if (tripleSlashSkill && tripleSlashSkill.IsTripleSlashing())
        {
            rb.MovePosition(tripleSlashSkill.GetLockPosition());
        }
        else if (explosionSkill && explosionSkill.IsExploding())
        {
            rb.MovePosition(explosionSkill.GetLockPosition());
        }
        else
        {
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        isMovementLocked = true;
        attackLockPosition = rb.position;
        animator.SetBool("isAttacking", true);

        yield return new WaitForSeconds(attackDelay);

        if (currentForm == PlayerForm.Warrior)
        {
            warriorAttack.PerformAttack();
        }
        else if (currentForm == PlayerForm.Mage)
        {
            audioSource.PlayOneShot(attackSound);
            mageAttack.PerformAttack(lastMoveDirection);
        }

        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("isAttacking", false);
        isMovementLocked = false;
        yield return new WaitForSeconds(attackCooldown-0.3f);
        isAttacking = false;
    }

    private IEnumerator UseSkillCoroutine(float duration)
    {
        isUsingSkill = true;
        yield return new WaitForSeconds(duration);
        isUsingSkill = false;
    }

   public void SetForm(PlayerForm form)
    {
        currentForm = form;
        warriorAttack.enabled = (form == PlayerForm.Warrior);
        mageAttack.enabled = (form == PlayerForm.Mage);
    
        if (animator != null)
        {
            animator.runtimeAnimatorController = form == PlayerForm.Warrior ? warriorAnimator : mageAnimator;
        }
        if (formChangeEffect != null)
        {
            formChangeEffect.PlayEffect(form);
        }
    
      
    
        Debug.Log("Đã chuyển sang dạng: " + form);
        playerHealth.SetForm(form);
    }

    public void OnHit()
    {
        StartCoroutine(HitCooldownRoutine());
    }

    private IEnumerator HitCooldownRoutine()
    {
        isOnHitCooldown = true;
        yield return new WaitForSeconds(hitCooldown);
        isOnHitCooldown = false;
    }

    public Vector2 GetLastMoveDirection() => lastMoveDirection;
    public void SetMagicStaff()
    {
        if (PlayerInventory.Instance.GetItemCount("Staff") > 0)
        {
            hasMagicStaff = true;
        }
    }
    private IEnumerator FormChangeCooldownRoutine()
    {
        isFormChangeOnCooldown = true;
        yield return new WaitForSeconds(formChangeCooldown);
        isFormChangeOnCooldown = false;
    }
   
}