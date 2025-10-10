using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Vector3 point;
        public float stopTime = 1f;
    }

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Easy;

    public float speedEasy = 2f;
    public float speedMedium = 3f;
    public float speedHard = 4f;

    [Header("Platform logic")]
    public Waypoint[] waypoints;
    public int startIndex = 0;
    private bool baseIsGoingForward = true;

    private float speed;
    private bool isWaiting = false;

    private int currentIndex;
    private bool isGoingForward;

    private Vector3 lastPosition;
    private Vector3 startPosition;

    private Coroutine coroutineWait;

    private void Awake()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                speed = speedEasy;
                break;
            case Difficulty.Medium:
                speed = speedMedium;
                break;
            case Difficulty.Hard:
                speed = speedHard;
                break;
        }
    }

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;

        currentIndex = startIndex;
        isGoingForward = baseIsGoingForward;
    }

    void Update()
    {
        if (waypoints.Length == 0 || isWaiting)
            return;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].point, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[currentIndex].point) < 0.05f)
        {
            coroutineWait = StartCoroutine(WaitAtPoint(waypoints[currentIndex].stopTime));

            if (isGoingForward)
            {
                currentIndex++;

                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 2;
                    isGoingForward = false;
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
    }

    private IEnumerator WaitAtPoint(float duration)
    {
        isWaiting = true;

        yield return new WaitForSeconds(duration);

        isWaiting = false;
    }

    void LateUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(GetComponent<Collider2D>().bounds.center, GetComponent<Collider2D>().bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerMovement player = hit.GetComponent<PlayerMovement>();

                if (player != null && player.isGrounded)
                    hit.transform.position += transform.position - lastPosition;
            }
        }

        lastPosition = transform.position;
    }

    public void Refresh()
    {
        if (coroutineWait != null)
        {
            StopCoroutine(coroutineWait);
            coroutineWait = null;
        }

        isWaiting = false;

        transform.position = startPosition;
        lastPosition = startPosition;

        currentIndex = startIndex;
        isGoingForward = baseIsGoingForward;
    }
}