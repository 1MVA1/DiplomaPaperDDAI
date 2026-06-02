
using UnityEngine;


public class EnemyBullet : MonoBehaviour
{
    [Header("Main")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;

    [Header("Logic")]
    public float speed = 4f;
    public float homingStrength = 2f;

    private Vector2 direction;

    private Transform player;

    [Header("Sprites")]
    public Sprite[] sprites;

    [Header("Do Not Touch")]
    public bool isOn = true;


    public void Init(Diff diff, Vector2 dir, Transform player_)
    {
        sr.sprite = sprites[(int)diff];

        direction = dir.normalized;

        if (diff == Diff.Hard)
        {
            player = player_;
        }
    }

    void FixedUpdate()
    {
        if (!isOn)
        {
            return;
        }

        if (player != null)
        {
            direction = Vector2.Lerp(direction, ((Vector2)player.position - rb.position).normalized,
                homingStrength * Time.fixedDeltaTime).normalized;
        }

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall") || col.CompareTag("Ground")
            || col.CompareTag("Player") || col.CompareTag("PlayerBullet"))
        {
            Destroy(gameObject);
        }
    }
}
