
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class Saw : MonoBehaviour, IApplyDiff_Saw, IRefreshable
{
    [Header("Edit")]
    public Diff diff;
    [Range(1.5f, 5)]
    public float size = 1.5f;
    public bool isCycle = false;
    public Vector2[] points;

    private float rotationSpeed = 480f;

    [Header("Main")]
    public SpriteRenderer sr;

    [Header("Logic")]
    public float timeStop = 1f;
    public float speed = 1f;

    private int currentIndex = 0;
    private bool isGoingForward = true;
    private bool isWaiting;

    private Vector3 startPosition;
    private Coroutine coroutineWait;

    private bool isOn = false;

    [Header("Sprites")]
    public Sprite[] sprites;


    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall += () =>
        {
            if (this == null || gameObject == null)
            {
                return;
            }

            ApplyDiff(diff, size, speed, timeStop, 0, 0, points, isCycle);
        };
#endif
    }

    public void PrepDiff()
    {
        int idx = (int)diff;

        sr.sprite = sprites[idx];
    }
    public void ApplyDiff(Diff diff_, float size_, float speed_, float timeStop_, float acceleration_, 
        float maxSpeed_, Vector2[] points_, bool isCycle_)
    {
        diff = diff_;

        transform.localScale = new Vector3(size_, size_, 1f);
        points = points_;

        isCycle = isCycle_;

        speed = speed_;
        timeStop = timeStop_;

        PrepDiff();
    }

    void Start()
    {
        startPosition = transform.position;
    }
    void Update()
    {
        if (!isOn)
        {
            return;
        }

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (isWaiting)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[currentIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, points[currentIndex]) < 0.01f)
        {
            ChooseNextPoint();
        }
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
        transform.rotation = Quaternion.identity;

        currentIndex = 0;
        isGoingForward = true;
    }
}
