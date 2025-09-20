using UnityEngine;

public class AntiGravityField : MonoBehaviour
{
    public Difficulty difficulty = Difficulty.Easy;

    public float antiGravityScaleEasy = 0f;
    public float antiGravityScaleMedium = -0.25f;
    public float antiGravityScaleHard = -0.5f;

    private float antiGravityScale;

    private float normalGravityScale;

    private void Awake()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                antiGravityScale = antiGravityScaleEasy;
                break;
            case Difficulty.Medium:
                antiGravityScale = antiGravityScaleMedium;
                break;
            case Difficulty.Hard:
                antiGravityScale = antiGravityScaleHard;
                break;
        }
    }

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