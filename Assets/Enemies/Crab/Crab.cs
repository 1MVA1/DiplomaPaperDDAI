using Unity.VisualScripting;
using UnityEngine;

public class Crab : MonoBehaviour
{
    private Rigidbody2D rb;

    [System.Serializable]
    public class CrabDiff
    {
        public float patrolSpeed;
        public float chaseSpeed;

        public CrabDiff(float patrolSpeed, float chaseSpeed, float detectionRange)
        {
            this.patrolSpeed = patrolSpeed;
            this.chaseSpeed = chaseSpeed;
        }
    }

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Easy;

    public CrabDiff diffEasy = new CrabDiff(1.5f, 2f, 3f);
    public CrabDiff diffMedium = new CrabDiff(2.5f, 3.5f, 5f);
    public CrabDiff diffHard = new CrabDiff(3f, 5f, 7f);

    public float rangeMedium = 3f;
    public float rangeHard = 4f;

    private CrabDiff currentDiff;

    [Header("Movement")]
    public bool movingRight = false;

    [Header("Navigation")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;

    private Transform playerTransform;

    [Header("Colider")]
    public int segments = 10;

    private float radius;

    private bool canSeePlayer = false;

    private void Awake()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                currentDiff = diffEasy;

                break;

            case Difficulty.Medium:
                currentDiff = diffMedium;
                radius = rangeMedium;

                break;

            case Difficulty.Hard:
                currentDiff = diffHard;
                radius = rangeHard;

                break;
        }
    }

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();

        if (difficulty != Difficulty.Easy)
        {
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();

            Vector2[] points = new Vector2[segments + 2];
            points[0] = new Vector2(0f, -0.5f);

            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.PI * i / segments;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                points[i + 1] = new Vector2(x, y);
            }

            poly.points = points;
        }
        else
        {
            PolygonCollider2D poly = GetComponent<PolygonCollider2D>();

            if (poly != null) {
                Destroy(poly); 
            }
        }
    }

    void Update()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                Patrol();
                break;

            case Difficulty.Medium:
                MediumBehavior();
                break;

            case Difficulty.Hard:
                HardBehavior();
                break;
        }
    }

    private bool TryToGetPlayerTransform()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null) 
            {
                playerTransform = player.transform;

                return true;
            }

            return false;
        }

        return true;
    }

    private void Patrol() 
    {
        CheckNeedFlip();

        rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * currentDiff.patrolSpeed, rb.linearVelocity.y);
    }

    private void MediumBehavior()
    {
        if (TryToGetPlayerTransform())
        {
            if (canSeePlayer)
            {
                CheckNeedFlip();

                if (playerTransform.position.x > transform.position.x && movingRight) 
                {
                    rb.linearVelocity = new Vector2(currentDiff.chaseSpeed, rb.linearVelocity.y);

                    return;
                }
                else if (playerTransform.position.x < transform.position.x && !movingRight) 
                {
                    rb.linearVelocity = new Vector2(-currentDiff.chaseSpeed, rb.linearVelocity.y);

                    return;
                }
            }
        }

        Patrol();
    }

    private void HardBehavior()
    {
        if (TryToGetPlayerTransform() && canSeePlayer)
        {
            if (playerTransform.position.x > transform.position.x && !movingRight || 
                playerTransform.position.x < transform.position.x && movingRight) {
                Flip();
            }

            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

            if (groundInfo.collider != null) {
                rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * currentDiff.chaseSpeed, rb.linearVelocity.y);
            }
            else {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            return;
        }

        Patrol();
    }

    private void CheckNeedFlip()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        if (groundInfo.collider == false) {
            Flip();
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;

        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            canSeePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) {
            canSeePlayer = false;
        }
    }
}