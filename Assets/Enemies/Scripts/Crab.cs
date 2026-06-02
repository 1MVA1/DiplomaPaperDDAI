
using UnityEngine;
using System.Collections;


public class Crab : MonoBehaviour, IApplyDiff_Enemy, IRefreshable
{
    [Header("Edit")]
    public Diff diff;
    public bool isMovingRight = false;

    public float flipThreshold = 0.2f;
    public int segments = 10;
    public float radius = 3f;

    private int idx = 0;

    [Header("Main")]
    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public CapsuleCollider2D cColl;
    public PolygonCollider2D polColl;

    public LayerMask sightMask;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    public Transform groundCheck;
    public Transform wallCheck;

    public float groundCheckDistance = 0.25f;
    public float wallCheckDistance = 0.25f;

    [Header("Diff")]
    public int[] HPs = { 3, 4, 5 };
    public float[] speeds = { 0.8f, 1f, 1.1f };
    public float[] chaseSpeeds = { 1f, 1.2f, 1.3f };

    public float jumpForce = 3f;
    public float jumpCooldown = 1.5f;
    public float jumpAngle = 30f;

    private int HP;
    private float speed;
    private float chaseSpeed;

    private Vector3 startPosition;
    private bool startIsMovingRight;

    private Transform player = null;
    private bool canSeePlayer = false;

    private float jumpTimer = 0f;
    private bool isInJump = false;
    private Collider2D lastGroundCollider;

    private bool isAlive = true;
    private bool isOn = false;

    [Header("Sprites")]
    public Sprite[] sprites1;
    public Sprite[] sprites2;

    public Color damageColor;

    public float frequency = 0.4f;

    private float timer;
    private bool isFirstSprite = true;

    private Color originalColor;


    private void OnValidate()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            ApplyDiff(diff, isMovingRight);
        };
#endif
    }

    public void PrepDiff()
    {
        idx = (int)diff;
        sr.sprite = sprites1[idx];

        HP = HPs[idx];
        speed = speeds[idx];
        chaseSpeed = chaseSpeeds[idx];

        MakeFlip();

        DrawPolygonCollider();
    }
    public void ApplyDiff(Diff diff_, bool isMovingRight_)
    {
        diff = diff_;

        isMovingRight = isMovingRight_;

        PrepDiff();
    }

    void DrawPolygonCollider()
    {
        Vector2[] points = new Vector2[segments + 2];

        points[0] = Vector2.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI * i / segments;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            points[i + 1] = new Vector2(x, y);
        }

        polColl.points = points;
    }

    void Start()
    {
        startPosition = transform.position;
        startIsMovingRight = isMovingRight;

        originalColor = sr.color;
    }
    void Update()
    {
        if (!isOn || !isAlive)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= frequency)
        {
            isFirstSprite = !isFirstSprite;
            sr.sprite = isFirstSprite ? sprites1[idx] : sprites2[idx];

            timer = 0;
        }

        CheckCanSee();

        switch (diff)
        {
            case Diff.Easy:
                EasyBehavior();
            break;

            case Diff.Medium:
                MediumBehavior();
            break;

            case Diff.Hard:
                HardBehavior();
            break;
        }
    }

    private void CheckCanSee()
    {
        if (player == null)
        {
            canSeePlayer = false;
            return;
        }

        Transform[] points = { player.Find("CheckTop"), player.Find("CheckCenter"), player.Find("CheckBottom") };

        foreach (Transform point in points)
        {
            if (point == null)
            {
                continue;
            }

            Vector2 dir = point.position - transform.position;

            float dist = dir.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dist, sightMask);

            if (hit.collider == null)
            {
                canSeePlayer = true;
                return;
            }
        }

        canSeePlayer = false;
    }

    private bool CheckNeedFlip()
    {
        return CheckNeedFlip_Wall() || CheckNeedFlip_Ground();
    }
    private bool CheckNeedFlip_Wall()
    {
        Vector2 forwardDirection = isMovingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(wallCheck.position, forwardDirection, wallCheckDistance, wallLayer);
        Debug.DrawRay(wallCheck.position, forwardDirection * wallCheckDistance, Color.red);

        return wallInfo.collider != null;
    }
    private bool CheckNeedFlip_Ground()
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

        return groundInfo.collider == null;
    }

    private void MakeFlip()
    {
        if (isMovingRight)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    private void Flip()
    {
        if (CheckNeedFlip())
        {
            isMovingRight = !isMovingRight;

            MakeFlip();
        }
    }

    private void Patrol()
    {
        Flip();

        rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * speed, rb.linearVelocity.y);
    }
    private void EasyBehavior()
    {
        if (canSeePlayer)
        {
            Flip();

            if ((player.position.x > transform.position.x && isMovingRight) ||
                (player.position.x < transform.position.x && !isMovingRight))
            {
                rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * chaseSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * speed, rb.linearVelocity.y);
            }
        }
        else
        {
            Patrol();
        }
    }
    private void MediumBehavior()
    {
        if (canSeePlayer)
        {
            float distanceX = player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > flipThreshold && 
                (distanceX > 0 && !isMovingRight || distanceX < 0 && isMovingRight))
            {
                isMovingRight = !isMovingRight;

                MakeFlip();
            }

            if (CheckNeedFlip())
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * chaseSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            Patrol();
        }
    }
    private void HardBehavior()
    {
        if (canSeePlayer)
        {
            if (lastGroundCollider == null)
            {
                jumpTimer -= Time.deltaTime;
            }

            float distanceX = player.position.x - transform.position.x;

            if (Mathf.Abs(distanceX) > flipThreshold && (distanceX > 0 && !isMovingRight
                || distanceX < 0 && isMovingRight))
            {
                Flip();
            }

            if (jumpTimer <= 0f && IsPlayerAboveInAngle())
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

                lastGroundCollider = groundInfo.collider;

                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpTimer = jumpCooldown;
            }
            else if (!isInJump && lastGroundCollider)
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

                if (groundInfo.collider != lastGroundCollider)
                {
                    isInJump = true;
                }
            }
            else if (isInJump)
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

                if (groundInfo.collider == lastGroundCollider)
                {
                    isInJump = false;
                    lastGroundCollider = null;
                }
            }
            else
            {
                rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * chaseSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            if (!isInJump && lastGroundCollider)
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

                if (groundInfo.collider != lastGroundCollider)
                {
                    isInJump = true;
                }
            }
            else if (isInJump)
            {
                RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
                Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.red);

                if (groundInfo.collider == lastGroundCollider)
                {
                    isInJump = false;
                    lastGroundCollider = null;
                }
            }
            else
            {
                Patrol();
            }
        }
    }

    private bool IsPlayerAboveInAngle()
    {
        Vector2 localPos = transform.InverseTransformPoint(player.position);

        if (localPos.y <= 0f)
        {
            return false;
        }

        return Vector2.Angle(Vector2.up, localPos) <= jumpAngle;
    }

    public void TakeDamage()
    {
        HP -= 1;

        sr.color = damageColor;

        if (HP <= 0)
        {
            isAlive = false;

            StartCoroutine(DeathRoutine());
        }
        else
        {
            StartCoroutine(DamagRoutine());
        }
    }

    private IEnumerator DamagRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isOn)
        {
            yield break;
        }

        sr.color = originalColor;
    }
    private IEnumerator DeathRoutine()
    {
        cColl.enabled = false;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(Vector2.up * 2, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);

        if (!isOn)
        {
            yield break;
        }

        float duration = 0.35f;
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < duration)
        {
            if (!isOn)
            {
                yield break;
            }

            elapsed += Time.deltaTime;

            sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, elapsed / duration));

            yield return null;
        }

        rb.simulated = false;
        sr.enabled = false;
    }

    public void TurnOn()
    {
        isOn = true;

        rb.simulated = true;
        rb.gravityScale = 1f;
    }
    public void TurnOff()
    {
        isOn = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        rb.gravityScale = 0f;
    }
    public void Refresh()
    {
        isAlive = true;

        transform.position = startPosition;

        sr.color = originalColor;

        isMovingRight = startIsMovingRight;
        player = null;

        HP = HPs[idx];

        sr.enabled = true;
        cColl.enabled = true;

        canSeePlayer = false;

        MakeFlip();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }
}
