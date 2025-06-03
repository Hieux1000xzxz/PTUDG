using UnityEngine;
using System.Collections;
using NUnit.Framework.Internal.Commands;

public enum PlayerForm { Warrior, Mage }

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public float attackDelay = 0.4f;
    public float attackCooldown = 0.3f;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.right;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private WarriorAttack warriorAttack;
    private MageAttack mageAttack;

    private bool isAttacking = false;
    private bool isMovementLocked = false;
    private bool isUsingSkill = false;

    private Vector2 attackLockPosition;

    private PlayerForm currentForm;
    private TripleSlashSkill tripleSlashSkill;
    private MoonveilDashSkill moonveilDashSkill;
    private MageExplosionSkill explosionSkill;
    private IceBlastSkill iceBlastSkill;

    public RuntimeAnimatorController warriorAnimator;
    public RuntimeAnimatorController mageAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        warriorAttack = GetComponent<WarriorAttack>();
        mageAttack = GetComponent<MageAttack>();
        tripleSlashSkill = GetComponent<TripleSlashSkill>();
        moonveilDashSkill = GetComponent<MoonveilDashSkill>();
        explosionSkill = GetComponent<MageExplosionSkill>();
        iceBlastSkill = GetComponent<IceBlastSkill>();

    }
    private void Start()
    {
        // Mặc định là dạng Warrior
        SetForm(PlayerForm.Warrior);
    }
    private void Update()
    {
        if (!isMovementLocked)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput = moveInput.normalized;

            if (moveInput.sqrMagnitude > 0.01f)
            {
                lastMoveDirection = moveInput;
            }

            if (lastMoveDirection.x > 0.1f)
                spriteRenderer.flipX = false;
            else if (lastMoveDirection.x < -0.1f)
                spriteRenderer.flipX = true;

            animator.SetBool("isMoving", moveInput.sqrMagnitude > 0);
        }

        // Tấn công
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
        if (Input.GetKeyDown(KeyCode.L) && !isUsingSkill)
        {
            if (currentForm == PlayerForm.Warrior && tripleSlashSkill && !tripleSlashSkill.IsTripleSlashing())
            {
                StartCoroutine(UseSkillCoroutine(tripleSlashSkill.totalDuration));
                tripleSlashSkill.ActivateTripleSlash();
            }
            else if (currentForm == PlayerForm.Mage && iceBlastSkill && !iceBlastSkill.IsCasting())
            {
                StartCoroutine(UseSkillCoroutine(0.5f));
                iceBlastSkill.ActivateIceBlast();
            }
        }

        if (Input.GetKeyDown(KeyCode.K) && !isUsingSkill)
        {
            if (currentForm == PlayerForm.Warrior && moonveilDashSkill && !moonveilDashSkill.IsDashing())
            {
                StartCoroutine(UseSkillCoroutine(moonveilDashSkill.dashDuration));
                moonveilDashSkill.ActivateMoonveilDash();
            }
            else if (currentForm == PlayerForm.Mage && explosionSkill && !explosionSkill.IsExploding())
            {
                StartCoroutine(UseSkillCoroutine(explosionSkill.explosionDelay));
                explosionSkill.ActivateExplosion();
            }
        }

        // Biến hình
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetForm(PlayerForm.Warrior);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetForm(PlayerForm.Mage);
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
            Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
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
            mageAttack.PerformAttack(lastMoveDirection);
        }

        yield return new WaitForSeconds(attackCooldown);
        animator.SetBool("isAttacking", false);
        isMovementLocked = false;
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

        // Thay đổi Animator Controller theo form
        if (animator != null)
        {
            if (form == PlayerForm.Warrior && warriorAnimator != null)
                animator.runtimeAnimatorController = warriorAnimator;
            else if (form == PlayerForm.Mage && mageAnimator != null)
                animator.runtimeAnimatorController = mageAnimator;
        }

        Debug.Log("Đã chuyển sang dạng: " + form);
    }

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
}
