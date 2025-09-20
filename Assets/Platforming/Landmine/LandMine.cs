using UnityEngine;

public class Landmine : MonoBehaviour
{
    public CircleCollider2D activationCollider;
    public CircleCollider2D explosionCollider;

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

    public Difficulty difficulty = Difficulty.Easy;

    public BombDiff diffEasy = new BombDiff(3f, 4f, 2.25f);
    public BombDiff diffMedium = new BombDiff(3f, 5f, 1.75f);
    public BombDiff diffHard = new BombDiff(4f, 6f, 1.5f);

    private BombDiff currentDiff;

    public float disappearDelay = 0.5f;       

    private bool isActivated = false;

    private void Awake()
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

        if (activationCollider != null) {
            activationCollider.radius = currentDiff.activationRadius;
        }

        if (explosionCollider != null) {
            explosionCollider.radius = currentDiff.explosionRadius;
        }
    }

    void Start() {
        explosionCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActivated)
        {
            isActivated = true;

            activationCollider.enabled = false;
            gameObject.tag = "Death";

            Debug.Log("Tick");

            Invoke(nameof(Explode), currentDiff.explosionDelay);
        }
    }

    void Explode()
    {
        Debug.Log("Boom");

        explosionCollider.enabled = true;

        Invoke(nameof(Disappear), disappearDelay);
    }

    void Disappear() {
        Destroy(gameObject);
    }
}