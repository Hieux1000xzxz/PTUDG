using UnityEngine;
using System.Collections;

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
    private bool isCasting = false;

    private Vector2 attackLockPosition;

    private PlayerForm currentForm;
    private TripleSlashSkill tripleSlashSkill;
    private MoonveilDashSkill moonveilDashSkill;
    private MageExplosionSkill explosionSkill;


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

    }
    private void Start()
    {
        // Mặc định là dạng Warrior
        SetForm(PlayerForm.Warrior);
    }
    private void Update()
    {
        if (isAttacking) return;

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

        // Tấn công
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentForm == PlayerForm.Warrior && !isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
            else if (currentForm == PlayerForm.Mage && !isAttacking)
            {
                StartCoroutine(MageAttackRoutine());
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (currentForm == PlayerForm.Warrior && tripleSlashSkill && !tripleSlashSkill.IsTripleSlashing())
            {
                tripleSlashSkill.ActivateTripleSlash();
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (currentForm == PlayerForm.Warrior && moonveilDashSkill && !moonveilDashSkill.IsDashing())
            {
                moonveilDashSkill.ActivateMoonveilDash();
            }
            if(currentForm == PlayerForm.Mage && explosionSkill)
            {
                StartCoroutine(CastExplosionRoutine());
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
        else
        {
            Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
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
        isAttacking = false;
    }
    private IEnumerator MageAttackRoutine()
    {
        isAttacking = true;
        attackLockPosition = rb.position;

        animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f);

        if (currentForm == PlayerForm.Warrior)
        {
            warriorAttack.PerformAttack();
        }
        else if (currentForm == PlayerForm.Mage)
        {
            mageAttack.PerformAttack(lastMoveDirection);
        }
        yield return new WaitForSeconds(attackCooldown+0.2f);
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
    private IEnumerator CastExplosionRoutine()
    {
        moveInput = Vector2.zero;

        animator.SetTrigger("castSkill");
        yield return new WaitForSeconds(0.7f); // thời gian tung chiêu (khớp animation)

        explosionSkill.ActivateExplosion();

        yield return new WaitForSeconds(0.3f); // đợi thêm chút cho cảm giác mượt
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
