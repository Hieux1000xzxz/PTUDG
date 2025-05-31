using System.Collections.Generic;
using UnityEngine;

public class FreezeEffect : MonoBehaviour
{
    private float timer;
    private List<Behaviour> disabledComponents = new();
    private Rigidbody2D rb;

    public void Freeze(float freezeTime)
    {
        if (!CompareTag("Enemy")) return;

        timer = freezeTime;

        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (
                comp == this ||
                comp is Animator ||
                comp is SpriteRenderer ||
                comp is EnemyHealth
            )
                continue;

            if (comp.enabled)
            {
                comp.enabled = false;
                disabledComponents.Add(comp);
            }
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.Sleep();
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(0.5f, 0.9f, 1f, 1f);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            foreach (var comp in disabledComponents)
                comp.enabled = true;

            if (rb != null)
                rb.WakeUp();

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = Color.white;

            Destroy(this);
        }
    }
}
