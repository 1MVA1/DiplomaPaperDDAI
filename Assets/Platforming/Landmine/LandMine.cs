using UnityEngine;

public class Landmine : MonoBehaviour, IAdjustableDifficulty
{
    public CircleCollider2D activationCollider;
    public CircleCollider2D explosionCollider;

    private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class BombDiff
    {
        public float activationRadius;
        public float explosionRadius;
        public float explosionDelay;

        public BombDiff(float activationRadius, float explosionRadius, float explosionDelay)
        {
            this.activationRadius = activationRadius;
            this.explosionRadius = explosionRadius;
            this.explosionDelay = explosionDelay;
        }
    }

    [Header("Difficulty Settings")]
    public BombDiff diffEasy = new BombDiff(3f, 4f, 2.25f);
    public BombDiff diffMedium = new BombDiff(3f, 5f, 1.75f);
    public BombDiff diffHard = new BombDiff(4f, 6f, 1.5f);

    private BombDiff currentDiff;

    [Header("Platform logic")]
    public float disappearDelay = 0.1f;

    private bool isActivated = false;

    public void ApplyDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentDiff = diffEasy;
                break;
            case Difficulty.Medium:
                currentDiff = diffMedium;
                break;
            case Difficulty.Hard:
                currentDiff = diffHard;
                break;
        }

        if (activationCollider != null)
            activationCollider.radius = currentDiff.activationRadius;

        if (explosionCollider != null)
            explosionCollider.radius = currentDiff.explosionRadius;
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {
        explosionCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            activationCollider.enabled = false;
            gameObject.tag = "DeathTrigger";

            Debug.Log("Tick");

            Invoke(nameof(Explode), currentDiff.explosionDelay);
        }
    }

    void Explode()
    {
        Debug.Log("Boom");

        explosionCollider.enabled = true;

        Invoke(nameof(HideMine), disappearDelay);
    }

    void HideMine()
    {
        gameObject.tag = "Untagged";

        spriteRenderer.enabled = false;

        explosionCollider.enabled = false;
    }

    public void Refresh()
    {
        isActivated = false;

        spriteRenderer.enabled = true;

        activationCollider.enabled = true;
        explosionCollider.enabled = false;
    }
}