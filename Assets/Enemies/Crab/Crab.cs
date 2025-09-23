using UnityEngine;

public class Crab : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    public float patrolSpeed = 2f;      
    public float chaseSpeed = 4f;

    public bool movingRight = false;

    [Header("Navigation")]
    public LayerMask groundLayer;
    public Transform groundCheck;        
    public float groundCheckDistance = 1f;
    
    public float detectionRange = 5f;

    private Transform playerTransform;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (TryToGetPlayerTransform())
        {

            RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

            if (groundInfo.collider == false) 
            {
                movingRight = !movingRight;

                Vector3 scale = transform.localScale;
                scale.x *= -1;

                transform.localScale = scale;
            }

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (playerTransform.position.x > transform.position.x && movingRight) {
                    rb.linearVelocity = new Vector2(chaseSpeed, rb.linearVelocity.y);
                }
                else if (playerTransform.position.x < transform.position.x && !movingRight) {
                    rb.linearVelocity = new Vector2(-chaseSpeed, rb.linearVelocity.y);
                }
                else {
                    rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * patrolSpeed, rb.linearVelocity.y);
                }
            }
            else {
                rb.linearVelocity = new Vector2((movingRight ? 1 : -1) * patrolSpeed, rb.linearVelocity.y);
            }
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
}