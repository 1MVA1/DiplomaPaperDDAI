using UnityEngine;

public class AntiGravityField : MonoBehaviour
{
    public Difficulty trapDifficulty = Difficulty.Easy;

    public float antiGravityScale = -1f;

    private float normalGravityScale;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                normalGravityScale = rb.gravityScale;
                rb.gravityScale = antiGravityScale;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null) {
                rb.gravityScale = normalGravityScale;
            }
        }
    }
}