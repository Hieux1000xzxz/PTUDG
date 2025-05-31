using UnityEngine;

public class SlashMover : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 8f;

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyHealth>()?.TakeDamage(15);
        }
    }
}
