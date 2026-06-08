
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooter : MonoBehaviour, IApplyDiff_Enemy, IRefreshable
{
    [Header("Edit")]
    public Diff diff;
    public bool isMovingRight = false;

    public int segments = 10;
    public float radius = 3f;

    public float flipThreshold = 0.2f;

    private int idx = 0;

    [Header("Main")]
    public GameObject bulletPrefab;
    public Rigidbody2D rb;
    public SpriteRenderer srBody;
    public SpriteRenderer srLegs;
    public CapsuleCollider2D cColl;
    public PolygonCollider2D polColl;
    public Transform shootTransform;

    public LayerMask sightMask;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    public Transform groundCheck;
    public Transform wallCheck;

    public float groundCheckDistance = 0.25f;
    public float wallCheckDistance = 0.25f;

    [Header("Diff")]
    public float[] speeds = { 0.8f, 1f, 1.1f };
    public float fireRate = 1.5f;
    public float shootAnimTime = 0.5f;

    private Vector3 startPosition;
    private bool startIsMovingRight;

    private int HP = 3;
    private float speed;

    private bool isAlive = true;
    private bool isOn = false;

    private float lastTimeShoot;

    private Transform player = null;
    private bool canSeePlayer = false;

    private Coroutine shootAnimCoroutine;

    private List<EnemyBullet> bullets = new();

    [Header("Sprites")]
    public Sprite[] spritesHead1;
    public Sprite[] spritesHead2;
    public Sprite[] spritesLegs1;
    public Sprite[] spritesLegs2;

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
        srBody.sprite = spritesHead1[idx];
        srLegs.sprite = spritesLegs1[idx];

        speed = speeds[idx];

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

        originalColor = srBody.color;
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
            srLegs.sprite = isFirstSprite ? spritesLegs1[idx] : spritesLegs2[idx];

            timer = 0;
        }

        CheckCanSee();

        if (diff == Diff.Easy)
        {
            EasyBehavior();
        }
        else
        {
            MediumHardBehavior();
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
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
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

    private void Shoot()
    {
        if (Time.time >= lastTimeShoot + fireRate)
        {
            lastTimeShoot = Time.time;

            shootAnimCoroutine = StartCoroutine(ShootAnimationRoutine());

            EnemyBullet bullet = Instantiate(bulletPrefab, shootTransform.position, 
                shootTransform.rotation).GetComponent<EnemyBullet>();

            bullet.Init(diff, (player.position - shootTransform.position).normalized, player);
            bullets.Add(bullet);
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
                Shoot();

                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
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
    private void MediumHardBehavior()
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
                rb.linearVelocity = new Vector2((isMovingRight ? 1 : -1) * speed, rb.linearVelocity.y);
            }

            Shoot();
        }
        else
        {
            Patrol();
        }
    }

    public void TakeDamage()
    {
        HP -= 1;

        srBody.color = damageColor;
        srLegs.color = damageColor;

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

    private IEnumerator ShootAnimationRoutine()
    {
        srBody.sprite = spritesHead2[idx];

        yield return new WaitForSeconds(shootAnimTime);

        srBody.sprite = spritesHead1[idx];
        shootAnimCoroutine = null;
    }
    private IEnumerator DamagRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (!isOn)
        {
            yield break;
        }

        srBody.color = originalColor;
        srLegs.color = originalColor;
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
        Color startColor = srBody.color;

        while (elapsed < duration)
        {
            if (!isOn)
            {
                yield break;
            }

            elapsed += Time.deltaTime;

            float a = Mathf.Lerp(1f, 0f, elapsed / duration);

            srBody.color = new Color(startColor.r, startColor.g, startColor.b, a);
            srLegs.color = new Color(startColor.r, startColor.g, startColor.b, a);

            yield return null;
        }

        rb.simulated = false;
        srBody.enabled = false;
        srLegs.enabled = false;
    }

    public void TurnOn()
    {
        isOn = true;

        rb.simulated = true;
    }
    public void TurnOff()
    {
        isOn = false;

        rb.simulated = false;

        foreach (EnemyBullet bullet in bullets)
        {
            if (bullet != null)
            {
                bullet.isOn = false;
            }
        }
    }
    public void Refresh()
    {
        isAlive = true;

        transform.position = startPosition;

        srBody.color = originalColor;
        srLegs.color = originalColor;

        isMovingRight = startIsMovingRight;
        player = null;

        HP = 3;

        srBody.enabled = true;
        srLegs.enabled = true;
        cColl.enabled = true;

        canSeePlayer = false;

        MakeFlip();

        foreach (EnemyBullet bullet in bullets)
        {
            if (bullet != null)
            {
                Destroy(bullet.gameObject);
            }
        }
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
