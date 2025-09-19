using UnityEngine;

public class Trap : MonoBehaviour
{
    public CircleCollider2D deadlyCollider;

    public Difficulty trapDifficulty = Difficulty.Easy;

    public float explosionDelay = 2f;   
    public float disappearDelay = 0.5f;       

    private bool isActivated = false;

    void Start() {
        deadlyCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated)
        {
            Debug.Log("Tick");
            isActivated = true;
            Invoke(nameof(Explode), explosionDelay);
        }
    }

    void Explode()
    {
        Debug.Log("Boom");

        deadlyCollider.enabled = true;

        Invoke(nameof(Disappear), disappearDelay);
    }

    void Disappear() {
        Destroy(gameObject);
    }
}