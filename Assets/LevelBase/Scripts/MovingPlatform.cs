
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MovingPlatform : MonoBehaviour, IRefreshable
{
#if UNITY_EDITOR
    public bool queued = false;
#endif

    [Header("Edit")]
    [Range(2, 5)]
    public int size = 2;
    public bool isCycle = false;
    public Vector2[] points;

    private float spacing = 0.54f;

    [Header("Main")]
    public GameObject prefab;
    public BoxCollider2D boxCol;

    [Header("Logic")]
    public float speed = 0.5f;
    public float timeStop = 1f;

    private int currentIndex = 0;
    private bool isGoingForward = true;
    private bool isWaiting = false;

    private Vector3 startPosition;
    private Coroutine coroutineWait;
    private Vector3 lastPosition;

    private bool isOn = false;

    [Header("Sprites")]
    public Sprite[] sprites;


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (queued)
        {
            return;
        }

        queued = true;

        EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            Generate();

            queued = false;
        };
#endif
    }
    private void Generate()
    {
        ClearChildren();

        boxCol.size = new Vector3(size * spacing, boxCol.size.y, 1.0f);

        int count = Mathf.Max(1, Mathf.RoundToInt(size));
        float centerOffset = (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            GameObject obj;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            }
            else
            {
                obj = Instantiate(prefab, transform);
            }
#else
        obj = Instantiate(prefab, transform);
#endif

            float x = (i - centerOffset) * spacing;
            obj.transform.localPosition = new Vector3(x, 0f, 0f);

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (i == 0)
            {
                sr.sprite = sprites[0];
            }
            else if (i == count - 1)
            {
                sr.sprite = sprites[0];
                obj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            }
            else
            {
                sr.sprite = sprites[1];
            }
        }

#if UNITY_EDITOR
        queued = false;
#endif
    }
    private void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Destroy(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
    }

    public void PrepDiff()
    {
    }

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
    }
    void Update()
    {
        if (!isOn || isWaiting)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[currentIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[currentIndex]) < 0.01f)
        {
            ChooseNextPoint();
        }
    }
    void LateUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCol.bounds.center, boxCol.bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("PlayerLegs"))
            {
                PlayerMovement player = hit.GetComponent<PlayerMovement>();

                if (player != null && player.isGrounded)
                {
                    hit.transform.position += transform.position - lastPosition;
                }
            }
        }

        lastPosition = transform.position;
    }

    void ChooseNextPoint()
    {
        coroutineWait = StartCoroutine(WaitAtPoint(timeStop));

        if (isGoingForward)
        {
            currentIndex++;

            if (currentIndex >= points.Length)
            {
                if (isCycle)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex = points.Length - 2;
                    isGoingForward = false;
                }
            }
        }
        else
        {
            currentIndex--;

            if (currentIndex < 0)
            {
                currentIndex = 1;
                isGoingForward = true;
            }
        }
    }
    private IEnumerator WaitAtPoint(float duration)
    {
        isWaiting = true;

        yield return new WaitForSeconds(duration);

        isWaiting = false;
    }

    public void TurnOn()
    {
        ChooseNextPoint();

        isOn = true;
    }
    public void TurnOff()
    {
        isOn = false;
    }
    public void Refresh()
    {
        if (coroutineWait != null)
        {
            StopCoroutine(coroutineWait);

            coroutineWait = null;
        }

        transform.position = startPosition;
        lastPosition = startPosition;

        currentIndex = 0;
        isGoingForward = true;
    }
}
