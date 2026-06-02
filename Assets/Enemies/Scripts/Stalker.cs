
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Stalker : MonoBehaviour, IApplyDiff_Enemy, IRefreshable
{
    [Header("Edit")]
    public Diff diff;
    public bool isMovingRight = false;

    public float flipThreshold = 0.2f;

    private int idx = 0;

    [Header("Main")]
    public SpriteRenderer sr;
    public CapsuleCollider2D cColl;

    [Header("Diff")]
    public int[] HPs = { 3, 4, 5 };
    public float[] speeds = { 0.5f, 0.75f, 0.75f };
    public float[] transparencies = { 1f, 0.7f, 0.4f };

    private float speed = 0f;
    private int HP;

    private Vector3 startPosition;
    private bool isMovingRightStart;

    private Coroutine startDelayCoroutine;

    private Transform playerTransform;

    private bool canSeePlayer = false;
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
        if (PrefabUtility.IsPartOfPrefabAsset(gameObject)) return;

        EditorApplication.delayCall += () =>
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
        speed = speeds[idx];

        var color = sr.color;
        color.a = transparencies[idx];
        sr.color = color;

        HP = HPs[idx];

        MakeFlip();
    }
    public void ApplyDiff(Diff diff_, bool isMovingRight_)
    {
        diff = diff_;

        isMovingRight = isMovingRight_;

        PrepDiff();
    }

    void Start()
    {
        startPosition = transform.position;
        isMovingRightStart = isMovingRight;

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

        if (!canSeePlayer || playerTransform == null)
        {
            return;
        }

        transform.position += (playerTransform.position - transform.position).normalized * speed * Time.deltaTime;

        float distanceX = playerTransform.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > flipThreshold && (distanceX > 0 && !isMovingRight 
            || distanceX < 0 && isMovingRight))
        {
            isMovingRight = !isMovingRight;

            MakeFlip();
        }
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

    public void TakeDamage()
    {
        HP -= 1;

        sr.color = damageColor;

        canSeePlayer = true;

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

        sr.enabled = false;
    }

    public void TurnOn()
    {
        GameObject playerGO = GameObject.Find("Player(Clone)");

        if (playerGO == null)
        {
            Debug.LogError("Stalker: Player not found on scene!");
            return;
        }

        playerTransform = playerGO.transform;

        isOn = true;
    }
    public void TurnOff()
    {
        isOn = false;
    }
    public void Refresh()
    {
        isAlive = true;

        timer = 0;
        isFirstSprite = true;

        sr.sprite = sprites1[idx];

        sr.color = originalColor;

        HP = HPs[idx];

        transform.position = startPosition;
        isMovingRight = isMovingRightStart;

        canSeePlayer = false;

        sr.enabled = true;
        cColl.enabled = true;

        MakeFlip();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canSeePlayer = true;
        }
    }
}
