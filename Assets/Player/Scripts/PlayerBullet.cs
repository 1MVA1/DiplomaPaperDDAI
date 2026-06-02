
using UnityEngine;


public class PlayerBullet : MonoBehaviour
{
    [Header("Main")]
    public Rigidbody2D rb;

    [Header("Logic")]
    public float speed = 10f;

    private Vector2 direction;


    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall") || col.CompareTag("Ground") || col.CompareTag("Enemy")
            || col.CompareTag("EnemyBullet"))
        {
            Destroy(gameObject);
        }
    }
}
