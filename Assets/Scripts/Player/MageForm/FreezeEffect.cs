using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[DisallowMultipleComponent]
public class FreezeEffect : MonoBehaviour
{
    [SerializeField] private Color freezeColor = new Color(0.5f, 0.9f, 1f, 1f);
    [SerializeField] private float freezeDuration = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Color originalColor;
    private Vector2 originalVelocity;
    private float originalAngularVelocity;
    private bool originalKinematic;
    private List<MonoBehaviour> disabledComponents = new List<MonoBehaviour>();
    private Coroutine freezeCoroutine;
    private Vector3 frozenPosition;

    private void Awake()
    {
        CacheComponents();
    }

    public void Freeze(float duration)
    {
        if (!CompareTag("Enemy")) return;

        // Nếu đã đóng băng thì reset thời gian
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }

        freezeDuration = duration;
        freezeCoroutine = StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        SetupFreeze();
        float timer = freezeDuration;
        while (timer > 0f)
        {
            transform.position = frozenPosition;
            timer -= Time.deltaTime;
            yield return null;
        }

        Unfreeze();
    }

    private void SetupFreeze()
    {
        frozenPosition = transform.position;

        // Disable all scripts except this one
        foreach (var component in GetComponents<MonoBehaviour>())
        {
            if (component != this && component.enabled)
            {
                disabledComponents.Add(component);
                component.enabled = false;
            }
        }

        // Freeze physics
        if (rb != null)
        {
            originalVelocity = rb.linearVelocity;
            originalAngularVelocity = rb.angularVelocity;
            originalKinematic = rb.bodyType == RigidbodyType2D.Kinematic; // Updated to use bodyType
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic; // Updated to use bodyType
        }

        // Pause animation
        if (animator != null)
        {
            animator.speed = 0f;
        }

        // Change color
        if (sr != null)
        {
            originalColor = sr.color;
            sr.color = freezeColor;
        }
    }

    private void Unfreeze()
    {
        // Re-enable components
        foreach (var component in disabledComponents)
        {
            if (component != null)
            {
                component.enabled = true;
            }
        }
        disabledComponents.Clear();

        // Restore physics
        if (rb != null)
        {
            rb.bodyType = originalKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic; // Updated to use bodyType
            if (!originalKinematic)
            {
                rb.linearVelocity = originalVelocity;
                rb.angularVelocity = originalAngularVelocity;
            }
        }

        // Resume animation
        if (animator != null)
        {
            animator.speed = 1f;
        }

        // Restore color
        if (sr != null)
        {
            sr.color = originalColor;
        }

        Destroy(this);
    }

    private void CacheComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
        }
    }
}